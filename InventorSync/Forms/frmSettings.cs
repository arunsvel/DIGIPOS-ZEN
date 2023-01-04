using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using DigiposZen.InventorBL.Master;
using DigiposZen.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DigiposZen
{
	public partial class frmSettings : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmSettings(int iSettingsID = 0, bool bFromEdit = false)
		{
            Cursor.Current = Cursors.WaitCursor;
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

            iIDFromEditWindow = iSettingsID;
            cboCasing.SelectedIndex = 0;
            bFromEditWindowSettings = bFromEdit;
            this.BackColor = Global.gblFormBorderColor;
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        Common Comm = new Common();
        UspGetStateInfo GetState = new UspGetStateInfo();
        clsState clsSt = new clsState();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        bool bFromEditWindowSettings;
        int iIDFromEditWindow;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        //Drag Form
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
        //For Help
        private void txtOrganisationName_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtOrganisationName, "Please specify Organisation Name");
        }
        private void txtAddress_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtAddress, "Please specify Address");
        }
        private void txtStreet_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtStreet, "Please specify Street");
        }
        private void txtContact_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtContact, "Please specify Contact Number");
        }
        private void txtEmail_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtEmail, "Please specify Email");
        }
        private void txtTaxRegNo_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtTaxRegNo, "Please specify Tax Registration No");
        }
        private void txtECommNo_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtECommNo, "Please specify E-Commerce Number");
        }
        private void txtStateCode_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(cmbState, "Please specify State Code");
        }
        private void txtBarcodePrefix_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtBarcodePrefix, "Please specify Barcode Prefix");
        }
        private void txtMajorCurr_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtMajorCurr, "Please specify Major Currency");
        }
        private void txtMinorCurr_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtMinorCurr, "Please specify Minor Currency");
        }
        private void txtMajCurrSymbol_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtMajCurrSymbol, "Please specify Major Currency Symbol");
        }
        private void txtMinCurrSymbol_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtMinCurrSymbol, "Please specify Minor Currency Symbol");
        }
        private void txtCurrDecimalpoints_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtCurrDecimalpoints, "Please specify Currency Decimal Points");
        }
        private void txtQtyDecimalPoints_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtQtyDecimalPoints, "Please specify Quantity Decimal Points");
        }
        private void txtSRate1_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtSRate1, "Please specify Sales Rate1 Name to show particular form");
        }
        private void txtSRate2_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtSRate2, "Please specify Sales Rate2 Name to show particular form");
        }
        private void txtSRate3_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtSRate3, "Please specify Sales Rate3 Name to show particular form");
        }
        private void txtSRate4_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtSRate4, "Please specify Sales Rate4 Name to show particular form");
        }
        private void txtSRate5_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtSRate5, "Please specify Sales Rate5 Name to show particular form");
        }
        private void txtMRP_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtMRP, "Please specify MRP Name to show particular form");
        }
        private void txtBackUpPath1_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtBackUpPath1, "Please select Back Up Path1");
        }
        private void txtBackUpPath2_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtBackUpPath2, "Please select Back Up Path2");
        }
        private void txtBackUpPath3_Click(object sender, EventArgs e)
        {
            ToolTipSettings.SetToolTip(txtBackUpPath3, "Please select Back Up Path3");
        }

        private void rdoProfile_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(1);
            txtOrganisationName.Focus();
        }
        private void rdoMasters_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(4);
            tbtnCostCenterConf.Focus();
        }
        private void rdoTaxInfo_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(2);
            tbtnTaxSettings.Focus();
        }
        private void rdoAdvanced_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(3);
            tbtnBarcode.Focus();
        }
        private void rdoThemes_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(5);
            tbtnThemeReq.Focus();
        }
        private void rdoCurrency_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(6);
            txtMajorCurr.Focus();
        }
        private void rdoAccounts_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(7);
            tbtnInsteadofDrisTo.Focus();
        }
        private void rdoBackups_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(8);
            tbtnShowBackuponExit.Focus();
        }
        private void rdoPriceList_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(9);
            txtSRate1.Focus();
        }
        private void btnPath1_Click(object sender, EventArgs e)//Backup Path
        {
            try
            {
                FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
                folderBrowserDlg.ShowNewFolderButton = false;
                folderBrowserDlg.SelectedPath = Application.StartupPath + "\\BackUp\\";
                DialogResult dlgResult = folderBrowserDlg.ShowDialog();

                if (dlgResult.Equals(DialogResult.OK))
                {
                    txtBackUpPath1.Text = folderBrowserDlg.SelectedPath;
                    Environment.SpecialFolder rootFolder = folderBrowserDlg.RootFolder;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void btnPath2_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
                folderBrowserDlg.ShowNewFolderButton = false;
                folderBrowserDlg.SelectedPath = Application.StartupPath + "\\BackUp\\";
                DialogResult dlgResult = folderBrowserDlg.ShowDialog();

                if (dlgResult.Equals(DialogResult.OK))
                {
                    txtBackUpPath2.Text = folderBrowserDlg.SelectedPath;
                    Environment.SpecialFolder rootFolder = folderBrowserDlg.RootFolder;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void btnPath3_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
                folderBrowserDlg.ShowNewFolderButton = false;
                folderBrowserDlg.SelectedPath = Application.StartupPath + "\\BackUp\\";
                DialogResult dlgResult = folderBrowserDlg.ShowDialog();

                if (dlgResult.Equals(DialogResult.OK))
                {
                    txtBackUpPath3.Text = folderBrowserDlg.SelectedPath;
                    Environment.SpecialFolder rootFolder = folderBrowserDlg.RootFolder;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void tbtnShowBackuponExit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoAccounts.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnAutobackupinLogin.Focus();
        }
        private void tbtnAutobackupinLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnShowBackuponExit.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                if (tbtnAutobackupinLogin.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    txtBackUpPath1.Focus();
                }
                else
                {
                    btnSave_Click(sender, e);
                }
            }
                
        }
        private void txtBackUpPath1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnAutobackupinLogin.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtBackUpPath2.Focus();
        }
        private void txtBackUpPath2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtBackUpPath1.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtBackUpPath3.Focus();
        }
        private void txtBackUpPath3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtBackUpPath2.Focus();
            else if (e.KeyCode == Keys.Enter)
                btnSave.PerformClick();
        }
        private void txtOrganisationName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtAddress.Focus();
        }
        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtOrganisationName.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtStreet.Focus();
        }
        private void txtStreet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtAddress.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtContact.Focus();
        }
        private void txtContact_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtStreet.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtEmail.Focus();
        }
        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtContact.Focus();
            else if (e.KeyCode == Keys.Enter)
                dtpFinyearStart.Focus();
        }
        private void dtpFinyearStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtEmail.Focus();
            else if (e.KeyCode == Keys.Enter)
                dtpFinYearEnd.Focus();
        }
        private void dtpFinYearEnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                dtpFinyearStart.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoMasters.Focus();
        }
        private void tbtnTaxSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoMasters.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                if (tbtnTaxSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    rdoNoTax.Focus();
                }
                else
                {
                    tbtnTaxCollectedSource.Focus();
                }
            }
        }
        private void rdoNoTax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnTaxSettings.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoVAT.Focus();
        }
        private void rdoVAT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoNoTax.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoGST.Focus();
        }
        private void rdoGST_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoVAT.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtTaxRegNo.Focus();
        }
        private void txtTaxRegNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoGST.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtECommNo.Focus();
        }
        private void txtECommNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtTaxRegNo.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                cmbState.Focus();
                SendKeys.Send("{F4}");
            }
        }
       
        private void rdoNoCess_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up)
                cmbState.Focus();
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                rdoUpcomingCess.Focus();
        }
        private void rdoUpcomingCess_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up)
                rdoNoCess.Focus();
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                rdoUpComingCess2.Focus();
        }
        private void rdoUpComingCess2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up)
                rdoUpcomingCess.Focus();
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                tbtnTaxCollectedSource.Focus();
        }
        private void tbtnTaxCollectedSource_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (tbtnTaxSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    rdoUpComingCess2.Focus();
                }
                else
                {
                    tbtnTaxSettings.Focus();
                }
            }
            else if (e.KeyCode == Keys.Enter)
                rdoPriceList.Focus();
        }
        private void tbtnAdvanced_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoPriceList.Focus();
            else  if (e.KeyCode == Keys.Enter)
            {
                if (tbtnAdvanced.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    txtBarcodePrefix.Focus();
                }
                else
                {
                    tbtnExternalDeviceConn.Focus();
                }
            }
                
        }
        private void txtBarcodePrefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnAdvanced.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnExternalDeviceConn.Focus();
        }
        private void tbtnExternalDeviceConn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (tbtnAdvanced.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    txtBarcodePrefix.Focus();
                }
                else
                {
                    tbtnTaxSettings.Focus();
                }
            }
            else if (e.KeyCode == Keys.Enter)
                rdoThemes.Focus();
        }
        private void tbtnCostCenterConf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoProfile.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnCustArea.Focus();
        }
        private void tbtnCustArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnCostCenterConf.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnAgentCommForSales.Focus();
        }
        private void tbtnAgentCommForSales_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnCustArea.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnOffersandLoyalty.Focus();
        }
        private void tbtnOffersandLoyalty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnAgentCommForSales.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnDiscGrouping.Focus();
        }
        private void tbtnDiscGrouping_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnOffersandLoyalty.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnSize.Focus();
        }
        private void tbtnSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnDiscGrouping.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnColor.Focus();
        }
        private void tbtnColor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnSize.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnBrand.Focus();
        }
        private void tbtnBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnColor.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                cboCasing.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void tbtnThemeReq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoAdvanced.Focus();
            else if (e.KeyCode == Keys.Enter)
                cboThemes.Focus();
        }
        private void cboThemes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnThemeReq.Focus();
            else if (e.KeyCode == Keys.Enter)
                cboFont.Focus();
        }
        private void tbtnDarkMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboThemes.Focus(); //btnNextTheme.Focus();
            else if (e.KeyCode == Keys.Enter)
                btnSave.PerformClick();
        }
        private void txtMajorCurr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoThemes.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtMinorCurr.Focus();
        }
        private void txtMinorCurr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtMajorCurr.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtMajCurrSymbol.Focus();
        }
        private void txtMajCurrSymbol_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtMinorCurr.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtMinCurrSymbol.Focus();
        }
        private void txtMinCurrSymbol_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtMajCurrSymbol.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtCurrDecimalpoints.Focus();
        }
        private void txtCurrDecimalpoints_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtMinCurrSymbol.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnLakhsorMill.Focus();
        }
        private void txtQtyDecimalPoints_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnLakhsorMill.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoAccounts.Focus();
        }
        private void tbtnInsteadofDrisTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoCurrency.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnVerticalAccFormats.Focus();
        }
        private void tbtnVerticalAccFormats_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnInsteadofDrisTo.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoBackups.Focus();
        }
        private void tbtnLakhsorMill_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtCurrDecimalpoints.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtQtyDecimalPoints.Focus();
        }
        private void cboFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboThemes.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbarHeadingFont.Focus();
        }
        private void tbarHeadingFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboFont.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbarNormalFont.Focus();
        }
        private void tbarNormalFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbarHeadingFont.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbarDescFont.Focus();
        }
        private void tbarDescFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbarNormalFont.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoCurrency.Focus();
        }
        private void cboCasing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnBrand.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                rdoTaxInfo.Focus();
            }
        }
        private void chkSRate1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoTaxInfo.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtSRate1.Focus();
        }
        private void txtSRate1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                chkSRate1.Focus();
            else if (e.KeyCode == Keys.Enter)
                chkSRate2.Focus();
        }
        private void chkSRate2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtSRate1.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                if (chkSRate2.Checked == true)
                    txtSRate2.Focus();
                else
                    chkSRate3.Focus();
            }
        }
        private void txtSRate2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                chkSRate2.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                chkSRate3.Focus();
            }
        }
        private void chkSRate3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (chkSRate2.Checked == true)
                    txtSRate2.Focus();
                else
                    chkSRate2.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (chkSRate3.Checked == true)
                    txtSRate3.Focus();
                else
                    chkSRate4.Focus();
            }
        }
        private void txtSRate3_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Shift == true && e.KeyCode == Keys.Enter)
                chkSRate3.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                chkSRate4.Focus();
            }
        }
        private void chkSRate4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (chkSRate3.Checked == true)
                    txtSRate3.Focus();
                else
                    chkSRate3.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (chkSRate4.Checked == true)
                    txtSRate4.Focus();
                else
                    chkSRate5.Focus();
            }
        }
        private void txtSRate4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                chkSRate4.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                chkSRate5.Focus();
            }
        }
        private void chkSRate5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (chkSRate4.Checked == true)
                    txtSRate4.Focus();
                else
                    chkSRate4.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (chkSRate5.Checked == true)
                    txtSRate5.Focus();
                else
                    txtMRP.Focus();
            }
        }
        private void txtSRate5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                chkSRate5.Focus();
            else if (e.KeyCode == Keys.Enter)
                txtMRP.Focus();
        }
        private void txtMRP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (chkSRate5.Checked == true)
                    txtSRate5.Focus();
                else
                    chkSRate5.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                rdoAdvanced.Focus();
            }
        }
        private void textbox_KeyPress(Object sender, KeyPressEventArgs e)
        {
            //Allow Numeric and decimal point only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        //For Tab
        private void rdoBackups_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnShowBackuponExit.Focus();
            }
        }
        private void tbtnShowBackuponExit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnAutobackupinLogin.Focus();
            }
        }
        private void tbtnAutobackupinLogin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnPath1.Focus();
            }
        }
        private void btnPath1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnPath2.Focus();
            }
        }
        private void btnPath2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnPath3.Focus();
            }
        }
        private void btnPath3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void rdoAccounts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnInsteadofDrisTo.Focus();
            }
        }
        private void tbtnInsteadofDrisTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnVerticalAccFormats.Focus();
            }
        }
        private void tbtnVerticalAccFormats_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void txtMinCurrSymbol_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtCurrDecimalpoints.Focus();
            }
        }
        private void txtCurrDecimalpoints_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnLakhsorMill.Focus();
            }
        }
        private void tbtnLakhsorMill_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtQtyDecimalPoints.Focus();
            }
        }
        private void txtQtyDecimalPoints_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                chkSRate1.Focus();
            }
        }
        private void tbarDescFont_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void tbtnOffersandLoyalty_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnDiscGrouping.Focus();
            }
        }
        private void tbtnDiscGrouping_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnSize.Focus();
            }
        }
        private void tbtnBrand_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboCasing.Focus();
            }
        }
        private void cboCasing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void tbtnAdvanced_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtBarcodePrefix.Focus();
            }
        }
        private void txtBarcodePrefix_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnExternalDeviceConn.Focus();
            }
        }
        private void tbtnExternalDeviceConn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                //tabPage3.Focus();
            }
        }
        private void rdoTaxInfo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnTaxSettings.Focus();
            }
        }
        private void tbtnTaxSettings_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                rdoNoTax.Focus();
            }
        }
        private void rdoNoTax_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                rdoVAT.Focus();
            }
        }
        private void rdoVAT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                rdoGST.Focus();
            }
        }
        private void rdoGST_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtTaxRegNo.Focus();
            }
        }
        private void txtTaxRegNo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtECommNo.Focus();
            }
        }
        private void txtECommNo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cmbState.Focus();
            }
        }
        private void txtStateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                rdoNoCess.Focus();
            }
        }
        private void rdoNoCess_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                rdoUpcomingCess.Focus();
            }
        }
        private void rdoUpcomingCess_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                rdoUpComingCess2.Focus();
            }
        }
        private void rdoUpComingCess2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                tbtnTaxCollectedSource.Focus();
            }
        }
        private void tbtnTaxCollectedSource_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void dtpFinYearEnd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void tabPage3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                //tabPage4.Focus();
            }
        }
        private void tabPage4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.PerformClick();
            }
        }
        private void txtMajorCurr_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMajorCurr, true, false);
        }
        private void txtMajorCurr_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMajorCurr, false, false);
        }
        private void txtMinorCurr_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMinorCurr, true, false);
        }
        private void txtMinorCurr_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMinorCurr, false, false);
        }
        private void txtMajCurrSymbol_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMajCurrSymbol, true, false);
        }
        private void txtMajCurrSymbol_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMajCurrSymbol, false, false);
        }
        private void txtMinCurrSymbol_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMinCurrSymbol, true, false);
        }
        private void txtMinCurrSymbol_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMinCurrSymbol, false, false);
        }
        private void txtCurrDecimalpoints_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCurrDecimalpoints, true, false);
        }
        private void txtCurrDecimalpoints_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCurrDecimalpoints, false, false);
        }
        private void tbtnLakhsorMill_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnLakhsorMill, true, false);
        }
        private void tbtnLakhsorMill_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnLakhsorMill, false, false);
        }
        private void txtQtyDecimalPoints_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtQtyDecimalPoints, true, false);
        }
        private void txtQtyDecimalPoints_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtQtyDecimalPoints, false, false);
        }
        private void txtSRate1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate1, true, false);
        }
        private void txtSRate1_Leave(object sender, EventArgs e)
        {
            if (txtSRate1.Text.Length > 0)
            {
                if ((txtSRate1.Text.TrimStart().TrimEnd() == txtSRate2.Text.TrimStart().TrimEnd()) || (txtSRate1.Text.TrimStart().TrimEnd() == txtSRate3.Text.TrimStart().TrimEnd()) || (txtSRate1.Text.TrimStart().TrimEnd() == txtSRate4.Text.TrimStart().TrimEnd()) || (txtSRate1.Text.TrimStart().TrimEnd() == txtSRate5.Text.TrimStart().TrimEnd()))
                {
                    MessageBox.Show(txtSRate1.Text + " is already used in another PriceList.Please check...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSRate1.Focus();
                }
            }
            Comm.ControlEnterLeave(txtSRate1, false, false);
        }
        private void txtSRate3_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate3, true, false);
        }
        private void txtSRate3_Leave(object sender, EventArgs e)
        {
            if (txtSRate3.Text.Length > 0)
            {
                if ((txtSRate3.Text.TrimStart().TrimEnd() == txtSRate1.Text.TrimStart().TrimEnd()) || (txtSRate3.Text.TrimStart().TrimEnd() == txtSRate2.Text.TrimStart().TrimEnd()) || (txtSRate3.Text.TrimStart().TrimEnd() == txtSRate4.Text.TrimStart().TrimEnd()) || (txtSRate3.Text.TrimStart().TrimEnd() == txtSRate5.Text.TrimStart().TrimEnd()))
                {
                    MessageBox.Show(txtSRate3.Text + " is already used in another PriceList.Please check...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSRate3.Focus();
                }
            }

            Comm.ControlEnterLeave(txtSRate3, false, false);

        }
        private void txtSRate2_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate2, true, false);
        }
        private void txtSRate2_Leave(object sender, EventArgs e)
        {
            if (txtSRate2.Text.Length > 0)
            {
                if ((txtSRate2.Text.TrimStart().TrimEnd() == txtSRate1.Text.TrimStart().TrimEnd()) || (txtSRate2.Text.TrimStart().TrimEnd() == txtSRate3.Text.TrimStart().TrimEnd()) || (txtSRate2.Text.TrimStart().TrimEnd() == txtSRate4.Text.TrimStart().TrimEnd()) || (txtSRate2.Text.TrimStart().TrimEnd() == txtSRate5.Text.TrimStart().TrimEnd()))
                {
                    MessageBox.Show(txtSRate2.Text + " is already used in another PriceList.Please check...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSRate2.Focus();
                }
            }
            Comm.ControlEnterLeave(txtSRate2, false, false);
        }
        private void txtSRate4_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate4, true, false);
        }
        private void txtSRate4_Leave(object sender, EventArgs e)
        {
            if (txtSRate4.Text.Length > 0)
            {
                if ((txtSRate4.Text.TrimStart().TrimEnd() == txtSRate1.Text.TrimStart().TrimEnd()) || (txtSRate4.Text.TrimStart().TrimEnd() == txtSRate2.Text.TrimStart().TrimEnd()) || (txtSRate4.Text.TrimStart().TrimEnd() == txtSRate3.Text.TrimStart().TrimEnd()) || (txtSRate4.Text.TrimStart().TrimEnd() == txtSRate5.Text.TrimStart().TrimEnd()))
                {
                    MessageBox.Show(txtSRate4.Text + " is already used in another PriceList.Please check...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSRate4.Focus();
                }
            }
            Comm.ControlEnterLeave(txtSRate4, false, false);
        }
        private void txtSRate5_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate5, true, false);
        }
        private void txtSRate5_Leave(object sender, EventArgs e)
        {
            if (txtSRate5.Text.Length > 0)
            {
                if ((txtSRate5.Text.TrimStart().TrimEnd() == txtSRate1.Text.TrimStart().TrimEnd()) || (txtSRate5.Text.TrimStart().TrimEnd() == txtSRate2.Text.TrimStart().TrimEnd()) || (txtSRate5.Text.TrimStart().TrimEnd() == txtSRate3.Text.TrimStart().TrimEnd()) || (txtSRate5.Text.TrimStart().TrimEnd() == txtSRate4.Text.TrimStart().TrimEnd()))
                {
                    MessageBox.Show(txtSRate5.Text + " is already used in another PriceList.Please check...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSRate5.Focus();
                }
            }
            Comm.ControlEnterLeave(txtSRate5, false, false);
        }
        private void txtMRP_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMRP, true, false);
        }
        private void txtMRP_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMRP, false, false);
        }
        private void chkSRate1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate1, true, false);
        }
        private void chkSRate1_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate1, false, false);
        }
        private void chkSRate2_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate2, true, false);
        }
        private void chkSRate2_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate2, false, false);
        }
        private void chkSRate3_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate3, true, false);
        }
        private void chkSRate3_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate3, false, false);
        }
        private void chkSRate4_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate4, true, false);
        }
        private void chkSRate4_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate4, false, false);
        }
        private void chkSRate5_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate5, true, false);
        }
        private void chkSRate5_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(chkSRate5, false, false);
        }
        private void tbtnShowBackuponExit_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnShowBackuponExit, true, false);
        }
        private void tbtnShowBackuponExit_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnShowBackuponExit, false, false);
        }
        private void tbtnAutobackupinLogin_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAutobackupinLogin, true, false);
        }
        private void tbtnAutobackupinLogin_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAutobackupinLogin, false, false);
        }
        private void txtBackUpPath1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBackUpPath1, true, false);
        }
        private void txtBackUpPath1_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBackUpPath1, false, false);
        }
        private void txtBackUpPath2_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBackUpPath2, true, false);
        }
        private void txtBackUpPath2_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBackUpPath2, false, false);
        }
        private void txtBackUpPath3_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBackUpPath3, true, false);
        }
        private void txtBackUpPath3_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBackUpPath3, false, false);
        }
        private void tbtnInsteadofDrisTo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnInsteadofDrisTo, true, false);
        }
        private void tbtnInsteadofDrisTo_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnInsteadofDrisTo, false, false);
        }
        private void tbtnVerticalAccFormats_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnVerticalAccFormats, true, false);
        }
        private void tbtnVerticalAccFormats_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnVerticalAccFormats, false, false);
        }
        private void tbtnThemeReq_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnThemeReq, true, false);
        }
        private void tbtnThemeReq_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnThemeReq, false, false);
        }
        private void cboThemes_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboThemes, true, false);
        }
        private void cboThemes_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboThemes, false, false);
        }
        private void cboFont_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboFont, true, false);
        }
        private void cboFont_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboFont, false, false);
        }
        private void tbtnCostCenterConf_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnCostCenterConf, true, false);
        }
        private void tbtnCostCenterConf_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnCostCenterConf, false, false);
        }
        private void tbtnCustArea_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnCustArea, true, false);
        }
        private void tbtnCustArea_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnCustArea, false, false);
        }
        private void tbtnAgentCommForSales_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAgentCommForSales, true, false);
        }
        private void tbtnAgentCommForSales_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAgentCommForSales, false, false);
        }
        private void tbtnOffersandLoyalty_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnOffersandLoyalty, true, false);
        }
        private void tbtnOffersandLoyalty_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnOffersandLoyalty, false, false);
        }
        private void tbtnDiscGrouping_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnDiscGrouping, true, false);
        }
        private void tbtnDiscGrouping_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnDiscGrouping, false, false);
        }
        private void tbtnSize_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSize, true, false);
        }
        private void tbtnSize_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSize, false, false);
        }
        private void tbtnColor_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnColor, true, false);
        }
        private void tbtnColor_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnColor, false, false);
        }
        private void tbtnBrand_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnBrand, true, false);
        }
        private void tbtnBrand_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnBrand, false, false);
        }
        private void cboCasing_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboCasing, true, false);
        }
        private void cboCasing_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboCasing, false, false);
        }
        private void tbtnAdvanced_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAdvanced, true, false);
        }
        private void tbtnAdvanced_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAdvanced, false, false);
        }
        private void txtBarcodePrefix_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBarcodePrefix, true, false);
        }
        private void txtBarcodePrefix_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBarcodePrefix, false, false);
        }
        private void tbtnExternalDeviceConn_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnExternalDeviceConn, true, false);
        }
        private void tbtnExternalDeviceConn_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnExternalDeviceConn, false, false);
        }
        private void tbtnTaxSettings_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxSettings, true, false);
        }
        private void tbtnTaxSettings_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxSettings, false, false);
        }
        private void rdoNoTax_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoNoTax, true, false);
        }
        private void rdoNoTax_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoNoTax, false, false);
        }
        private void rdoVAT_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoVAT, true, false);
        }
        private void rdoVAT_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoVAT, false, false);
        }
        private void rdoGST_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoGST, true, false);
        }
        private void rdoGST_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoGST, false, false);
        }
        private void txtTaxRegNo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTaxRegNo, true, false);
        }
        private void txtTaxRegNo_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTaxRegNo, false, false);
        }
        private void txtECommNo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtECommNo, true, false);
        }
        private void txtECommNo_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtECommNo, false, false);
        }
        private void txtStateCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cmbState, true, false);
        }
        private void txtStateCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cmbState, false, false);
        }
        private void rdoNoCess_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoNoCess, true, false);
        }
        private void rdoNoCess_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoNoCess, false, false);
        }
        private void rdoUpcomingCess_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoUpcomingCess, true, false);
        }
        private void rdoUpcomingCess_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoUpcomingCess, false, false);
        }
        private void rdoUpComingCess2_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoUpComingCess2, true, false);
        }
        private void rdoUpComingCess2_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoUpComingCess2, false, false);
        }
        private void tbtnTaxCollectedSource_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxCollectedSource, true, false);
        }
        private void tbtnTaxCollectedSource_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxCollectedSource, false, false);
        }
        private void txtOrganisationName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtOrganisationName, true, false);
        }
        private void txtOrganisationName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtOrganisationName, false, false);
        }
        private void txtAddress_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress, true, false);
        }
        private void txtAddress_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress, false, false);
        }
        private void txtStreet_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtStreet, true, false);
        }
        private void txtStreet_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtStreet, false, false);
        }
        private void txtContact_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtContact, true, false);
        }
        private void txtContact_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtContact, false, false);
        }
        private void txtEmail_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEmail, true, false);
        }
        private void txtEmail_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEmail, false, false);
        }
        private void dtpFinyearStart_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(dtpFinyearStart, true, false);
        }
        private void dtpFinyearStart_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(dtpFinyearStart, false, false);
        }
        private void dtpFinYearEnd_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(dtpFinYearEnd, true, false);
        }
        private void dtpFinYearEnd_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(dtpFinYearEnd, false, false);
        }

        private void cboThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThemeSelection(cboThemes.SelectedIndex + 1);
        }
        private void chkSRate1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSRate1.Checked == true)
                chkSRate1.Checked = true;
            else
                chkSRate1.Checked = true;
        }
        private void chkSRate2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSRate2.Checked == true)
            {
                txtSRate2.Enabled = true;
                txtSRate2.Focus();
            }
            else
                txtSRate2.Enabled = false;
        }
        private void chkSRate3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSRate3.Checked == true)
            {
                txtSRate3.Enabled = true;
                txtSRate3.Focus();
            }
            else
                txtSRate3.Enabled = false;
        }
        private void chkSRate4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSRate4.Checked == true)
            {
                txtSRate4.Enabled = true;
                txtSRate4.Focus();
            }
            else
                txtSRate4.Enabled = false;
        }
        private void chkSRate5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSRate5.Checked == true)
            {
                txtSRate5.Enabled = true;
                txtSRate5.Focus();
            }
            else
                txtSRate5.Enabled = false;
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            this.Width = 623;
            this.Height = 663;
            pnlThemes.Visible = false;
            cboThemes.SelectedIndex = 0;
            tbtnLakhsorMill.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            dtpFinyearStart.Value = new DateTime(DateTime.Today.Year - 1, 4, 1);
            dtpFinYearEnd.Value = new DateTime(DateTime.Today.Year, 4, 1).AddDays(-1);
            FillFonts();
            LoadState();
            string sStateCode = AppSettings.StateCode;
            if (sStateCode == "")
                sStateCode = "32";
            else if (sStateCode == "0")
                sStateCode = "32";
            cmbState.SelectedValue = sStateCode;
            GetDataFromSettings();
            ShowFormsAsperClick(1);
            txtOrganisationName.Focus();
            txtOrganisationName.Select();


            if (tbtnBarcode.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
            {
                tbtnAdvanced.Enabled = true;
            }
            else
            {
                tbtnAdvanced.Enabled = false;
            }
        }
        private void frmSettings_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                   this.Close();
                }
                else if (e.KeyCode == Keys.F5)//Save
                {
                    btnSave.PerformClick();
                }
                else if (e.KeyCode == Keys.T && e.Control == true)//Save
                {
                    btnClearMasters.Visible = true;
                    btnclrTransaction.Visible = true;
                    button1.Visible = true;
                    button2.Visible = true;
                    btnUpdateDB.Visible = true;
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            int iCessmod = 0, iTaxmod = 0;

            if (rdoNoCess.Checked == true)
                iCessmod = 0;
            else if (rdoUpcomingCess.Checked == true)
                iCessmod = 1;
            else if (rdoUpComingCess2.Checked == true)
                iCessmod = 2;

            if (rdoNoTax.Checked == true)
                iTaxmod = 0;
            else if (rdoVAT.Checked == true)
                iTaxmod = 1;
            else if (rdoGST.Checked == true)
                iTaxmod = 2;

            if (string.IsNullOrEmpty(txtCurrDecimalpoints.Text.Trim()))
                txtCurrDecimalpoints.Text = "2";

            if (string.IsNullOrEmpty(txtQtyDecimalPoints.Text.Trim()))
                txtQtyDecimalPoints.Text = "2";

            Comm.SaveInAppSettings("STRWMIDENTIFIER", txtWMIdentifier.Text);
            Comm.SaveInAppSettings("STRWMBARCODELENGTH", txtWMBarcodeLength.Text);
            Comm.SaveInAppSettings("STRWMQTYLENGTH", txtWMQtyLength.Text);
            Comm.SaveInAppSettings("STRBATCODEPREFIXSUFFIX", txtBarcodePrefix.Text);
            Comm.SaveInAppSettings("MAJORCURRENCY", txtMajorCurr.Text);
            Comm.SaveInAppSettings("MINORCURRENCY", txtMinorCurr.Text);
            Comm.SaveInAppSettings("MAJORSYMBOL", txtMajCurrSymbol.Text);
            Comm.SaveInAppSettings("MINORSYMBOL", txtMinCurrSymbol.Text);
            Comm.SaveInAppSettings("BLNSHOWCOMPANYADDRESS", txtAddress.Text);
            Comm.SaveInAppSettings("BLNSHOWCOMPANYNAME", txtOrganisationName.Text);
            Comm.SaveInAppSettings("BLNTaxEnabled", IsActiveorInActive(tbtnTaxSettings));
            Comm.SaveInAppSettings("DBLCESS", IsCheckedOrUnChecked(rdoNoCess));
            Comm.SaveInAppSettings("BLNTOBYINDAYBOOK", IsActiveorInActive(tbtnInsteadofDrisTo));
            Comm.SaveInAppSettings("BLNVERTICALACCFORMAT", IsActiveorInActive(tbtnVerticalAccFormats));
            Comm.SaveInAppSettings("EXPLORERSKININDEX", (cboThemes.SelectedIndex + 1).ToString());
            Comm.SaveInAppSettings("BLNAUTOBACKUP", IsActiveorInActive(tbtnAutobackupinLogin));
            Comm.SaveInAppSettings("STRWINDOWFONT", "");
            Comm.SaveInAppSettings("STRWINDOWFONTSIZE", "");
            Comm.SaveInAppSettings("STRWINDOWFONTBOLD", "");
            Comm.SaveInAppSettings("STRWINDOWFONTFOCUSCOLOR", "");
            Comm.SaveInAppSettings("STRWINDOWFONTCOLOR", "");
            Comm.SaveInAppSettings("STRWINDOWFONTFOCUSTEXTCOLOR", "");
            Comm.SaveInAppSettings("BLNAGENT", IsActiveorInActive(tbtnAgentCommForSales)); // already there below
            Comm.SaveInAppSettings("INTCESSMODE", iCessmod.ToString());
            Comm.SaveInAppSettings("STRWINDOWBACKCOLOR", "");
            Comm.SaveInAppSettings("STRGRIDCOLOR", "");
            Comm.SaveInAppSettings("INTIMPLEMENTINGSTATECODE", cmbState.SelectedValue.ToString());
            Comm.SaveInAppSettings("MYGSTIN", txtTaxRegNo.Text.Trim());
            Comm.SaveInAppSettings("MYECOMMERCEGSTIN", txtECommNo.Text.Trim());
            Comm.SaveInAppSettings("AVAILABLETAXPER", txtAvailableTaxPercentages.Text.Trim());
            Comm.SaveInAppSettings("MyBGFontColor", "");
            Comm.SaveInAppSettings("MyContrastFontColor", "");
            Comm.SaveInAppSettings("MyBGColor", "");
            Comm.SaveInAppSettings("MyContrastColor", "");
            Comm.SaveInAppSettings("CurrencyDecimals", txtCurrDecimalpoints.Text.Trim());
            Comm.SaveInAppSettings("MajorCurrencySymbol", txtMajCurrSymbol.Text.Trim());
            Comm.SaveInAppSettings("MinorCurrencySymbol", txtMinCurrSymbol.Text.Trim());
            Comm.SaveInAppSettings("QtyDecimalFormat", txtQtyDecimalPoints.Text.Trim());
            Comm.SaveInAppSettings("BLNVERTICALACCFORMAT", IsActiveorInActive(tbtnVerticalAccFormats)); // dont know
            Comm.SaveInAppSettings("STRSTREET", txtStreet.Text);
            Comm.SaveInAppSettings("STRCONTACT", txtContact.Text);
            Comm.SaveInAppSettings("STEMAIL", txtEmail.Text);
            Comm.SaveInAppSettings("FSTARTDATE", dtpFinyearStart.Value.ToString("dd-MMM-yyyy"));
            Comm.SaveInAppSettings("FENDDATE", dtpFinYearEnd.Value.ToString("dd-MMM-yyyy"));
            Comm.SaveInAppSettings("BLNTAXCOLLSOURCE", IsActiveorInActive(tbtnTaxCollectedSource));
            Comm.SaveInAppSettings("BLNADVANCED", IsActiveorInActive(tbtnAdvanced));
            Comm.SaveInAppSettings("BLNBARCODE", IsActiveorInActive(tbtnBarcode));
            Comm.SaveInAppSettings("BLNAUTOPLU", IsActiveorInActive(tbtnPLUAuto));
            Comm.SaveInAppSettings("BLNEXTDEVCONN", IsActiveorInActive(tbtnExternalDeviceConn));
            Comm.SaveInAppSettings("BLNOFFERLOY", IsActiveorInActive(tbtnOffersandLoyalty));
            Comm.SaveInAppSettings("BLNDISCGROUP", IsActiveorInActive(tbtnDiscGrouping));
            //Comm.SaveInAppSettings("BLNMULTICATEGORY", IsActiveorInActive(tbtnMULTICATEGORY));
            Comm.SaveInAppSettings("BLNSRATEINC", IsActiveorInActive(tbtnSRateInc));
            Comm.SaveInAppSettings("BLNPRATEINC", IsActiveorInActive(tbtnPRateInc));
            Comm.SaveInAppSettings("BLNTEXSIZE", IsActiveorInActive(tbtnSize));
            Comm.SaveInAppSettings("BLNTEXCOLOR", IsActiveorInActive(tbtnColor));
            Comm.SaveInAppSettings("BLNTEXBRAND", IsActiveorInActive(tbtnBrand));
            Comm.SaveInAppSettings("BLNTHEME", IsActiveorInActive(tbtnThemeReq));
            Comm.SaveInAppSettings("STRLAKHORMILL", IsActiveorInActive(tbtnLakhsorMill));
            Comm.SaveInAppSettings("BLNSHOWBKONLOG", IsActiveorInActive(tbtnAutobackupinLogin));
            Comm.SaveInAppSettings("BLNAUTOBACKUPEXIT", IsActiveorInActive(tbtnShowBackuponExit));
            Comm.SaveInAppSettings("BLNCOSTCENTRE", IsActiveorInActive(tbtnCostCenterConf));
            Comm.SaveInAppSettings("StrBackupString", txtBackUpPath1.Text);
            Comm.SaveInAppSettings("StrBackupString2", txtBackUpPath2.Text);
            Comm.SaveInAppSettings("StrBackupString3", txtBackUpPath3.Text);
            Comm.SaveInAppSettings("INTCASINGID", cboCasing.SelectedIndex.ToString());

            Comm.SaveInAppSettings("FORMMAINBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpMain.BackColor));
            Comm.SaveInAppSettings("FORMHDRBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpHeader.BackColor));
            Comm.SaveInAppSettings("FORMFTRBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpFooter.BackColor));
            Comm.SaveInAppSettings("FORLFTBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpLeft.BackColor));
            Comm.SaveInAppSettings("FORMRHTBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpRight.BackColor));
            Comm.SaveInAppSettings("FORMHDRTXTCLR", System.Drawing.ColorTranslator.ToHtml(lblHeaderText.ForeColor));

            Comm.SaveInAppSettings("FORMHILTCLR1", System.Drawing.ColorTranslator.ToHtml(tblpHighLight1.BackColor));
            Comm.SaveInAppSettings("FORMHILTCLR2", System.Drawing.ColorTranslator.ToHtml(tblpHighLight2.BackColor));
            Comm.SaveInAppSettings("FORMHILTCLR3", System.Drawing.ColorTranslator.ToHtml(tblpHighLight3.BackColor));

            Comm.SaveInAppSettings("GRIDBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpGrid.BackColor));
            Comm.SaveInAppSettings("GRIDHDRBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpGridHeader.BackColor));
            Comm.SaveInAppSettings("GRIDHDRTXTCLR", System.Drawing.ColorTranslator.ToHtml(lblGridHeaderCol1.ForeColor));
            Comm.SaveInAppSettings("GRIDHDRTXTBLD", IsTrueorFalse(lblGridHeaderCol1.Font.Bold));
            Comm.SaveInAppSettings("GRIDHDRTXTFNT", lblGridHeaderCol1.Font.FontFamily.ToString());

            Comm.SaveInAppSettings("GRIDALTRWBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpGridAltRow.BackColor));
            Comm.SaveInAppSettings("GRIDALTRWTXTCLR", System.Drawing.ColorTranslator.ToHtml(lblGridAlternatCol1.ForeColor));
            Comm.SaveInAppSettings("GRIDALTRWTXTBLD", IsTrueorFalse(lblGridAlternatCol1.Font.Bold));
            Comm.SaveInAppSettings("GRIDALTRWTXTFNT", lblGridAlternatCol1.Font.FontFamily.ToString());

            Comm.SaveInAppSettings("GRIDSELRWBCKCLR", System.Drawing.ColorTranslator.ToHtml(tblpGridSelectedRow.BackColor));
            Comm.SaveInAppSettings("GRIDSELRWTXTCLR", System.Drawing.ColorTranslator.ToHtml(lblGridSelectCol1.ForeColor));
            Comm.SaveInAppSettings("GRIDSELRWTXTBLD", IsTrueorFalse(lblGridSelectCol1.Font.Bold));
            Comm.SaveInAppSettings("GRIDSELRWTXTFNT", lblGridSelectCol1.Font.FontFamily.ToString());

            Comm.SaveInAppSettings("GRIDNORRWTXTCLR", System.Drawing.ColorTranslator.ToHtml(lblGridNormalRow1.ForeColor));
            Comm.SaveInAppSettings("GRIDNORRWTXTBLD", IsTrueorFalse(lblGridNormalRow1.Font.Bold));
            Comm.SaveInAppSettings("GRIDNORRWTXTFNT", lblGridNormalRow1.Font.FontFamily.ToString());

            Comm.SaveInAppSettings("FONTFORAPP", cboFont.Text.ToString());
            Comm.SaveInAppSettings("HEADFNTSIZ", tbarHeadingFont.Value.ToString());
            Comm.SaveInAppSettings("NORFNTSIZ", tbarNormalFont.Value.ToString());
            Comm.SaveInAppSettings("DESCFNTSIZ", tbarDescFont.Value.ToString());
            
            Comm.SaveInAppSettings("PLCALCULATION", cboCalc.SelectedIndex.ToString());

            Comm.SaveInAppSettings("SRATE1ACT", IsChkboxCheckedOrUnChecked(chkSRate1));
            Comm.SaveInAppSettings("SRATE1NAME", txtSRate1.Text);
            Comm.SaveInAppSettings("SRATE2ACT", IsChkboxCheckedOrUnChecked(chkSRate2));
            Comm.SaveInAppSettings("SRATE2NAME", txtSRate2.Text);
            Comm.SaveInAppSettings("SRATE3ACT", IsChkboxCheckedOrUnChecked(chkSRate3));
            Comm.SaveInAppSettings("SRATE3NAME", txtSRate3.Text);
            Comm.SaveInAppSettings("SRATE4ACT", IsChkboxCheckedOrUnChecked(chkSRate4));
            Comm.SaveInAppSettings("SRATE4NAME", txtSRate4.Text);

            Comm.SaveInAppSettings("BLNCUSTAREA", IsActiveorInActive(tbtnCustArea));

            Comm.SaveInAppSettings("SRATE5ACT", IsChkboxCheckedOrUnChecked(chkSRate5));
            Comm.SaveInAppSettings("SRATE5NAME", txtSRate5.Text);
            Comm.SaveInAppSettings("MRPACT", IsChkboxCheckedOrUnChecked(chkMRP));
            Comm.SaveInAppSettings("MRPNAME", txtMRP.Text);
            Comm.SaveInAppSettings("INTTAXMODE", iTaxmod.ToString());

            string sQuery = "";

            try
            {
                sQuery = @"Alter Table tblCompanyMaster Add ParentCompanyID Numeric";
                Comm.fnExecuteNonQuery(sQuery, false);

                sQuery = @"Alter Table tblCompanyMaster Add ParentCompanyCode Varchar(500)";
                Comm.fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"Update tblCompanyMaster Set ParentCompanyID = (Select [Startup].[dbo].[tblCompany].[ParentID] From [Startup].[dbo].[tblCompany] Where [Startup].[dbo].[tblCompany].[CompanyCode]='" + AppSettings.CompanyCode + "') Where ParentCompanyID is null";
                Comm.fnExecuteNonQuery(sQuery, false);

                sQuery = @"Update tblCompanyMaster Set ParentCompanyCode = (Select [Startup].[dbo].[tblCompany].[CompanyCode] From [Startup].[dbo].[tblCompany] Where [Startup].[dbo].[tblCompany].[CompanyID]=tblCompanyMaster.ParentCompanyID) Where ParentCompanyCode is null";
                Comm.fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            Comm.MessageboxToasted("Settings", "Settings Saved successfully");
            Comm.LoadAppSettings();
            if (bFromEditWindowSettings == true)
            {
                this.Close();
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
           this.Close();
        }
        private void cmbState_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtECommNo.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoNoCess.Focus();
        }

        private void btnUpdateDB_Click(object sender, EventArgs e)
        {
            try
            {
                Comm.DBUpdate(true);
                Comm.CreateViewsAndProcudures();
                Comm.MessageboxToasted("System Update", "Database Updated successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnclrTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.gblUserName.ToUpper() == "ADMIN" || Global.gblUserName.ToUpper() == "DIGIPOS")
                {
                    if (MessageBox.Show("Are you sure to clear all transactions.", "Clear Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string sQuery = "";
                        sQuery = "EXEC CLEARTRANSACTIONS";//CLEARMASTERS
                        Comm.fnExecuteNonQuery(sQuery);
                        Comm.MessageboxToasted("Clear Transaction", "Transaction Cleared Successfully");
                    }
                }
                else
                {
                    MessageBox.Show("You are not authorised to clear transaction details.", "Clear Transaction", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region "METHODS -  -------------------------------------------- >>"
        //Description : Show Form when Radio Button Click
        private void ShowFormsAsperClick(int iColIndex = 1)
        {
            for (int g = 0; g < this.tblpForms.ColumnCount; g++)
            {
                if (iColIndex == g + 1)
                {
                    this.tblpForms.ColumnStyles[g].SizeType = SizeType.Percent;
                    this.tblpForms.ColumnStyles[g].Width = 100;
                }
                else
                {
                    this.tblpForms.ColumnStyles[g].SizeType = SizeType.Absolute;
                    this.tblpForms.ColumnStyles[g].Width = 0;
                }
            }
        }
        //Description : Theme Selection
        private void ThemeSelection(int iSelectionID = 1)
        {
            pnlThemes.Visible = true;
            lblHeaderText.Text = cboThemes.Text;
            if (iSelectionID == 1) // Theme 1 | Dark Gray - Existing Theme
            {
                tblpMain.BackColor = Color.White;
                tblpHeader.BackColor = Color.DimGray;
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#404040");
                tblpLeft.BackColor = Color.DimGray;
                tblpRight.BackColor = Color.DimGray;
                lblHeaderText.ForeColor = Color.White;

                tblpHighLight1.BackColor = System.Drawing.ColorTranslator.FromHtml("#fefff0");
                tblpHighLight2.BackColor = System.Drawing.ColorTranslator.FromHtml("#ebfcfe");
                tblpHighLight3.BackColor = System.Drawing.ColorTranslator.FromHtml("#e4f9e0");

                tblpGrid.BackColor = Color.White;
                tblpGridHeader.BackColor = Color.Silver;
                tblpGridAltRow.BackColor = Color.LightGray;
                tblpGridAltRow1.BackColor = Color.LightGray;
                tblpGridSelectedRow.BackColor = Color.Yellow;

                lblGridHeaderCol1.ForeColor = Color.DimGray;
                lblGridHeaderCol2.ForeColor = Color.DimGray;
                lblGridHeaderCol3.ForeColor = Color.DimGray;
                lblGridHeaderCol4.ForeColor = Color.DimGray;

                lblGridAlternatCol1.ForeColor = Color.Maroon;
                lblGridAlternatCol2.ForeColor = Color.Maroon;
                lblGridAlternatCol3.ForeColor = Color.Maroon;
                lblGridAlternatCol4.ForeColor = Color.Maroon;

                lblGridAlternat2Col1.ForeColor = Color.Maroon;
                lblGridAlternat2Col2.ForeColor = Color.Maroon;
                lblGridAlternat2Col3.ForeColor = Color.Maroon;
                lblGridAlternat2Col4.ForeColor = Color.Maroon;

                lblGridSelectCol1.ForeColor = Color.Black;
                lblGridSelectCol2.ForeColor = Color.Black;
                lblGridSelectCol3.ForeColor = Color.Black;
                lblGridSelectCol4.ForeColor = Color.Black;

                lblGridNormalRow1.ForeColor = Color.Black;
                lblGridNormalRow2.ForeColor = Color.Black;
                lblGridNormalRow3.ForeColor = Color.Black;
                lblGridNormalRow4.ForeColor = Color.Black;

            }
            else if (iSelectionID == 2) // Theme 2 | Light Blue
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#f1f7fd");
                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#e2f1ff");
                //tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#cde9ff");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#b5dbf9");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#e2f1ff");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#e2f1ff");

                lblHeaderText.ForeColor = Color.DimGray;
                lblFooterText.ForeColor = Color.DimGray;

                tblpHighLight1.BackColor = System.Drawing.ColorTranslator.FromHtml("#fefff0");
                tblpHighLight2.BackColor = System.Drawing.ColorTranslator.FromHtml("#cde9ff");
                tblpHighLight3.BackColor = System.Drawing.ColorTranslator.FromHtml("#e2f1ff");

                tblpGrid.BackColor = Color.White;
                tblpGridHeader.BackColor = Color.Silver;
                tblpGridAltRow.BackColor = Color.LightGray;
                tblpGridAltRow1.BackColor = Color.LightGray;
                tblpGridSelectedRow.BackColor = Color.Yellow;

                lblGridHeaderCol1.ForeColor = Color.DimGray;
                lblGridHeaderCol2.ForeColor = Color.DimGray;
                lblGridHeaderCol3.ForeColor = Color.DimGray;
                lblGridHeaderCol4.ForeColor = Color.DimGray;

                lblGridAlternatCol1.ForeColor = Color.Maroon;
                lblGridAlternatCol2.ForeColor = Color.Maroon;
                lblGridAlternatCol3.ForeColor = Color.Maroon;
                lblGridAlternatCol4.ForeColor = Color.Maroon;

                lblGridAlternat2Col1.ForeColor = Color.Maroon;
                lblGridAlternat2Col2.ForeColor = Color.Maroon;
                lblGridAlternat2Col3.ForeColor = Color.Maroon;
                lblGridAlternat2Col4.ForeColor = Color.Maroon;

                lblGridSelectCol1.ForeColor = Color.Black;
                lblGridSelectCol2.ForeColor = Color.Black;
                lblGridSelectCol3.ForeColor = Color.Black;
                lblGridSelectCol4.ForeColor = Color.Black;

                lblGridNormalRow1.ForeColor = Color.Black;
                lblGridNormalRow2.ForeColor = Color.Black;
                lblGridNormalRow3.ForeColor = Color.Black;
                lblGridNormalRow4.ForeColor = Color.Black;
            }
            else if (iSelectionID == 3) // Theme 3 | Baby Yellow
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#fefcf0");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff2ab");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff7d1");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff7d1");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff7d1");

                lblHeaderText.ForeColor = Color.DimGray;
                lblFooterText.ForeColor = Color.DimGray;
            }
            else if (iSelectionID == 4) // Theme 4 | Light Violet
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f3fc");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#e1c5fd");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#f2e6ff");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#f2e6ff");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#f2e6ff");

                lblHeaderText.ForeColor = Color.DimGray;
                lblFooterText.ForeColor = Color.DimGray;
            }
            else if (iSelectionID == 5) // Theme 5 | Light Pink
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#fdf4f8");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcce5");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffe4f1");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffe4f1");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffe4f1");

                lblHeaderText.ForeColor = Color.Maroon;
                lblFooterText.ForeColor = Color.DimGray;
            }
            else if (iSelectionID == 6) // Theme 6 | Pista Green
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#f3fbf1");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#cbf1c4");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#e4f9e0");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#e4f9e0");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#e4f9e0");

                lblHeaderText.ForeColor = Color.DarkGreen;
                lblFooterText.ForeColor = Color.DimGray;
            }
            else if (iSelectionID == 7) // Theme 7 | Light Gray
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#faf9f7");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#e1dfdd");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#f3f2f1");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#f3f2f1");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#f3f2f1");

                lblHeaderText.ForeColor = Color.Black;
                lblFooterText.ForeColor = Color.Black;
            }
            else if (iSelectionID == 8) // Theme 8 | Windows
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#fffbd9");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#f47537");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7c78c");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7c78c");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7c78c");

                lblHeaderText.ForeColor = Color.Black;
                lblFooterText.ForeColor = Color.White;
            }
            else if (iSelectionID == 9) // Theme 9 | Web
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#196fc4");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");

                lblHeaderText.ForeColor = Color.DimGray;
                lblFooterText.ForeColor = Color.White;
            }
            else if (iSelectionID == 10) // Theme 10 | Web
            {
                tblpMain.BackColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
                tblpFooter.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffd54f");

                tblpHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#00c8c8");
                tblpLeft.BackColor = System.Drawing.ColorTranslator.FromHtml("#00c8c8");
                tblpRight.BackColor = System.Drawing.ColorTranslator.FromHtml("#00c8c8");

                lblHeaderText.ForeColor = Color.White;
                lblFooterText.ForeColor = Color.DimGray;
            }
        }
        //Description : Fill Fonts
        private void FillFonts()
        {
            foreach (FontFamily oneFontFamily in FontFamily.Families)
            {
                cboFont.Items.Add(oneFontFamily.Name);
            }
        }
        //Description : Checking Toggle Button
        private string IsActiveorInActive(Syncfusion.Windows.Forms.Tools.ToggleButton tbtn)
        {
            string sRet = "";
            if (tbtn.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                sRet = "1";
            else
                sRet = "0";

            return sRet;
        }
        //Description : Polymorphism of Checking Toggle Button
        private void IsActiveorInActive(Syncfusion.Windows.Forms.Tools.ToggleButton tbtn, string sVal)
        {
            if (sVal == "0")
                tbtn.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            else
                tbtn.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
        }
        //Description : Checking RadioButton
        private string IsCheckedOrUnChecked(RadioButton rdo)
        {
            string sRet = "";
            if (rdo.Checked == true)
                sRet = "1";
            else
                sRet = "0";

            return sRet;
        }
        //Description : Polymorphism of Checking RadioButton
        private void IsCheckedOrUnChecked(RadioButton rdo, string sVal)
        {
            if (sVal == "0")
                rdo.Checked = false;
            else
                rdo.Checked = true;
        }
        //Description :Checking True or False
        private string IsTrueorFalse(bool bVal = true)
        {
            string sRet = "";
            if (bVal == true)
                sRet = "1";
            else
                sRet = "0";
            return sRet;
        }
        //Description : Checking CheckBox Value
        private string IsChkboxCheckedOrUnChecked(CheckBox chk)
        {
            string sRet = "";
            if (chk.Checked == true)
                sRet = "1";
            else
                sRet = "0";

            return sRet;
        }
        //Description : Polymorphism of Checking CheckBox Value
        private void IsChkboxCheckedOrUnChecked(CheckBox chk, string sVal)
        {
            if (sVal == "0")
                chk.Checked = false;
            else
                chk.Checked = true;
        }
        //Description : Get RGB for Theme 
        private int GetRGBFromDBString(string sDBStr, string sTyp = "A")
        {
            int iVal = 0;
            sDBStr = "Color[A = 255, R = 241, G = 247, B = 253]";
            string[] sSplit = sDBStr.Split(',');
            if (sSplit.Length > 0)
            {
                if (sTyp == "A")
                    iVal = Convert.ToInt32(sSplit[0].Substring(sSplit[0].Length - 3, 3));
                else if (sTyp == "R")
                    iVal = Convert.ToInt32(sSplit[1].Substring(sSplit[1].Length - 3, 3));
                else if (sTyp == "G")
                    iVal = Convert.ToInt32(sSplit[2].Substring(sSplit[2].Length - 3, 3));
                else if (sTyp == "B")
                    iVal = Convert.ToInt32(sSplit[3].Substring(sSplit[3].Length - 4, 3));
            }
            return iVal;
        }
        //Description : Get Data From Database
        private void GetDataFromSettings()
        {
            bool bGRIDHDRTXTBLD = false;
            bool bGRIDALTRWTXTBLD = false;
            bool bGRIDSELRWTXTBLD = false;
            bool bGRIDNORRWTXTBLD = false;

            DataTable dtGet = Comm.RetieveFromDBInAppSettings(Global.gblTenantID);
            if (dtGet != null)
            {
                for (int i = 0; i < dtGet.Rows.Count; i++)
                {
                    switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
                    {
                        case "STRWMIDENTIFIER":
                            txtWMIdentifier.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRWMBARCODELENGTH":
                            txtWMBarcodeLength.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRWMQTYLENGTH":
                            txtWMQtyLength.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBATCODEPREFIXSUFFIX":
                            txtBarcodePrefix.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORCURRENCY":
                            txtMajorCurr.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORCURRENCY":
                            txtMinorCurr.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORSYMBOL":
                            txtMajCurrSymbol.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORSYMBOL":
                            txtMinCurrSymbol.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWCOMPANYADDRESS":
                            txtAddress.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWCOMPANYNAME":
                            txtOrganisationName.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNTAXENABLED":
                            IsActiveorInActive(tbtnTaxSettings, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "DBLCESS":
                            IsCheckedOrUnChecked(rdoNoCess, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTOBYINDAYBOOK":
                            IsActiveorInActive(tbtnInsteadofDrisTo, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNVERTICALACCFORMAT":
                            IsActiveorInActive(tbtnVerticalAccFormats, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "EXPLORERSKININDEX":
                            cboThemes.SelectedIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()) - 1;
                            break;
                        //"DBLCESSONTAXPER": "" = dtGet.Rows[i]["ValueName"].ToString(); // already there above
                        case "BLNAUTOBACKUP":
                            IsActiveorInActive(tbtnAutobackupinLogin, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //case "STRWINDOWFONT": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "STRWINDOWFONTSIZE": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "STRWINDOWFONTBOLD": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "STRWINDOWFONTFOCUSCOLOR": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "STRWINDOWFONTCOLOR": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "STRWINDOWFONTFOCUSTEXTCOLOR": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //"BLNAGENT": "" = dtGet.Rows[i]["ValueName"].ToString(); // already there below
                        case "INTCESSMODE":
                            if (Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()) == 0)
                                rdoNoCess.Checked = true;
                            else if (Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()) == 1)
                                rdoUpcomingCess.Checked = true;
                            else
                                rdoUpComingCess2.Checked = true;

                            break;
                        //case "EnableWeighingMachine": "" = dtGet.Rows[i]["ValueName"].ToString();
                        case "BLNAGENT":
                            IsActiveorInActive(tbtnAgentCommForSales, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //case "STRWINDOWBACKCOLOR": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "STRGRIDCOLOR": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        case "INTIMPLEMENTINGSTATECODE":
                            cmbState.SelectedValue = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MYGSTIN":
                            txtTaxRegNo.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MYECOMMERCEGSTIN":
                            txtECommNo.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "AVAILABLETAXPER":
                            txtAvailableTaxPercentages.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        //case "MyBGFontColor": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "MyContrastFontColor": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "MyBGColor": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        //case "MyContrastColor": "" = dtGet.Rows[i]["ValueName"].ToString(); break;
                        case "CURRENCYDECIMALS":
                            txtCurrDecimalpoints.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORCURRENCYSYMBOL":
                            txtMajCurrSymbol.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORCURRENCYSYMBOL":
                            txtMinCurrSymbol.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "QTYDECIMALFORMAT":
                            txtQtyDecimalPoints.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        //"VerticalStatementsOfAccounts": "" = dtGet.Rows[i]["ValueName"].ToString(); // dont know
                        case "STRSTREET":
                            txtStreet.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRCONTACT":
                            txtContact.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STEMAIL":
                            txtEmail.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FSTARTDATE":
                            dtpFinyearStart.Value = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "FENDDATE":
                            dtpFinYearEnd.Value = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //case "BLNTAXSETT": "" = dtGet.Rows[i]["ValueName"].ToString(); //already there above
                        case "BLNTAXCOLLSOURCE":
                            IsActiveorInActive(tbtnTaxCollectedSource, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNBARCODE":
                            IsActiveorInActive(tbtnBarcode, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNADVANCED":
                            IsActiveorInActive(tbtnAdvanced, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNAUTOPLU":
                            IsActiveorInActive(tbtnPLUAuto, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNEXTDEVCONN":
                            IsActiveorInActive(tbtnExternalDeviceConn, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //"case BLNMULTBRANCH": "" = dtGet.Rows[i]["ValueName"].ToString(); // No Settings
                        case "BLNOFFERLOY":
                            IsActiveorInActive(tbtnOffersandLoyalty, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNDISCGROUP":
                            IsActiveorInActive(tbtnDiscGrouping, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //case "BLNMULTICATEGORY":
                        //    IsActiveorInActive(tbtnMULTICATEGORY, dtGet.Rows[i]["ValueName"].ToString());
                        //    break;
                        case "BLNSRATEINC":
                            IsActiveorInActive(tbtnSRateInc, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNPRATEINC":
                            IsActiveorInActive(tbtnPRateInc, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTEXSIZE":
                            IsActiveorInActive(tbtnSize, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTEXCOLOR":
                            IsActiveorInActive(tbtnColor, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTEXBRAND":
                            IsActiveorInActive(tbtnBrand, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTHEME":
                            IsActiveorInActive(tbtnThemeReq, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //case "BLNDARKMOD":
                        //    IsActiveorInActive(tbtnDarkMode, dtGet.Rows[i]["ValueName"].ToString());
                        //    break;
                        case "STRLAKHORMILL":
                            IsActiveorInActive(tbtnLakhsorMill, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNSHOWBKONLOG":
                            IsActiveorInActive(tbtnAutobackupinLogin, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNAUTOBACKUPEXIT":
                            IsActiveorInActive(tbtnShowBackuponExit, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        //"BLNPRICELIST": "" = dtGet.Rows[i]["ValueName"].ToString();
                        case "BLNCOSTCENTRE":
                            IsActiveorInActive(tbtnCostCenterConf, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "STRBACKUPSTRING":
                            txtBackUpPath1.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBACKUPSTRING2":
                            txtBackUpPath2.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBACKUPSTRING3":
                            txtBackUpPath3.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        //"StrBackupString4": "" = dtGet.Rows[i]["ValueName"].ToString();
                        case "INTCASINGID":
                            cboCasing.SelectedIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;

                        case "FORMMAINBCKCLR":
                            tblpMain.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(),"A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORMHDRBCKCLR":
                            tblpHeader.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORMFTRBCKCLR":
                            tblpFooter.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORLFTBCKCLR":
                            tblpLeft.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORMRHTBCKCLR":
                            tblpRight.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORMHDRTXTCLR":
                            lblHeaderText.ForeColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;

                        case "FORMHILTCLR1":
                            tblpHighLight1.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORMHILTCLR2":
                            tblpHighLight2.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "FORMHILTCLR3":
                            tblpHighLight3.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;

                        case "GRIDBCKCLR":
                            tblpGrid.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDHDRBCKCLR":
                            tblpGridHeader.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDHDRTXTCLR":
                            lblGridHeaderCol1.ForeColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDHDRTXTBLD":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "1")
                                bGRIDHDRTXTBLD = true;
                            else
                                bGRIDHDRTXTBLD = false;
                            break;
                        case "GRIDHDRTXTFNT":
                            if(bGRIDHDRTXTBLD == true)
                                lblGridHeaderCol1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Bold);
                            else
                                lblGridHeaderCol1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Regular);
                            break;

                        case "GRIDALTRWBCKCLR":
                            tblpGridAltRow.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDALTRWTXTCLR":
                            lblGridAlternatCol1.ForeColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDALTRWTXTBLD":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "1")
                                bGRIDALTRWTXTBLD = true;
                            else
                                bGRIDALTRWTXTBLD = false;
                            break;
                        case "GRIDALTRWTXTFNT":
                            if (bGRIDALTRWTXTBLD == true)
                                lblGridAlternatCol1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Bold);
                            else
                                lblGridAlternatCol1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Regular);
                            break;

                        case "GRIDSELRWBCKCLR":
                            tblpGridSelectedRow.BackColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDSELRWTXTCLR":
                            lblGridSelectCol1.ForeColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDSELRWTXTBLD":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "1")
                                bGRIDSELRWTXTBLD = true;
                            else
                                bGRIDSELRWTXTBLD = false;
                            break;
                        case "GRIDSELRWTXTFNT":
                            if (bGRIDSELRWTXTBLD == true)
                                lblGridSelectCol1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Bold);
                            else
                                lblGridSelectCol1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Regular);
                            break;

                          
                        case "GRIDNORRWTXTCLR":
                            lblGridNormalRow1.ForeColor = Color.FromArgb(GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "A"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "R"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "G"), GetRGBFromDBString(dtGet.Rows[i]["ValueName"].ToString(), "B"));
                            break;
                        case "GRIDNORRWTXTBLD":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "1")
                                bGRIDNORRWTXTBLD = true;
                            else
                                bGRIDNORRWTXTBLD = false;
                            break;
                        case "GRIDNORRWTXTFNT":
                            if (bGRIDNORRWTXTBLD == true)
                                lblGridNormalRow1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Bold);
                            else
                                lblGridNormalRow1.Font = new Font(dtGet.Rows[i]["ValueName"].ToString().Trim(), 10, FontStyle.Regular);
                            break;
                        case "FONTFORAPP":
                            cboFont.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "HEADFNTSIZ":
                            tbarHeadingFont.Value = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "NORFNTSIZ":
                            tbarNormalFont.Value = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "DESCFNTSIZ":
                            tbarDescFont.Value = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE1ACT":
                            IsChkboxCheckedOrUnChecked(chkSRate1, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "PLCALCULATION": cboCalc.SelectedIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE1NAME": txtSRate1.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE2ACT":
                            IsChkboxCheckedOrUnChecked(chkSRate2, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE2NAME": txtSRate2.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE3ACT": 
                            IsChkboxCheckedOrUnChecked(chkSRate3, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE3NAME": txtSRate3.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE4ACT": 
                            IsChkboxCheckedOrUnChecked(chkSRate4, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE4NAME":
                            txtSRate4.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNCUSTAREA":
                            IsActiveorInActive(tbtnCustArea, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE5ACT":
                            IsChkboxCheckedOrUnChecked(chkSRate5, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "SRATE5NAME":
                            txtSRate5.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MRPACT":
                            IsChkboxCheckedOrUnChecked(chkMRP, dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "MRPNAME":
                            txtMRP.Text = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "INTTAXMODE":
                            if (Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()) == 0)
                                rdoNoTax.Checked = true;
                            else if (Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()) == 1)
                                rdoVAT.Checked = true;
                            else
                                rdoGST.Checked = true;
                            break;
                    }
                }
            }
        }
        //Declaration: Load States and Statecode in combobox
        private void LoadState()
        {
            DataTable dtState = new DataTable();
            dtState = Comm.fnGetData("SELECT StateId,State+'-'+StateCode as StateCode From tblStates ORDER BY StateId Asc").Tables[0];
            cmbState.DataSource = dtState;
            cmbState.DisplayMember = "StateCode";
            cmbState.ValueMember = "StateId";
        }

        private void OldDBUpdate()
        {
            string sQuery = "";

            sQuery = "CREATE TABLE [dbo].[tblTransactionPause](" +
            "	[ID] [int] IDENTITY(1,1) NOT NULL, " +
            "	[VchTypeID] [numeric](18, 0) NULL," +
            "	[VchParentID] [numeric](18, 0) NULL," +
            "	[TransID] [numeric](18, 0) NULL," +
            "	[TransNo] [varchar](50) NULL," +
            "	[LastUpdateDt] [datetime] NULL," +
            "	[UpdateStatus] [int] NULL," +
            "	[JsonData] [varchar](max) NULL," +
            "	[TenantID] [int] NULL," +
            " CONSTRAINT [PK_tblTransactionPause] PRIMARY KEY CLUSTERED " +
            "(" +
            "	[ID] ASC" +
            ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
            ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetItemMaster')" +
            "DROP PROCEDURE UspGetItemMaster";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetItemMaster] " +
            "( " +
            "@ItemID    NUMERIC   (18,0), " +
            "@TenantID     NUMERIC   (18,0) " +
            ") " +
            "AS " +
            " BEGIN " +
            "	DECLARE @CatIDsNames	VARCHAR(1000) " +
            "	DECLARE @CatIDs	VARCHAR(1000) " +
            "     IF @ItemID <> 0  " +
            "     BEGIN " +
            "		 SELECT @CatIDs = CategoryIDs FROM tblItemMaster WHERE ItemID = @ItemID AND TenantID = @TenantID " +
            "		SELECT @CatIDsNames = COALESCE(@CatIDsNames + ',', '') + Category  " +
            "		FROM tblCategories  " +
            "		WHERE TenantID = @TenantID AND ','+ @CatIDs +',' LIKE '%,'+CONVERT(VARCHAR(50),CategoryID)+',%'; " +
            "         SELECT I.ItemID,ItemCode,ItemName,I.CategoryID,Description,ISNULL(PRate,0) as PRate,ISNULL(SrateCalcMode,0) as SrateCalcMode,CRateAvg,Srate1Per,I.SRate1,Srate2Per,I.SRate2,Srate3Per,I.SRate3,I.Srate4,Srate4Per,I.SRate5,Srate5Per,ISNULL(I.MRP,0) as MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,I.UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,PGID,ItemCodeUniCode,UPC,BatchMode,blnExpiry,Qty,MaxQty,IntNoOrWeight,I.SystemName,I.UserID,I.LastUpdateDate,I.LastUpdateTime,I.TenantID,blnCessOnTax,CompCessQty,CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,CessPer,VAT,CategoryIDs,ColorIDs,SizeIDs,BrandDisPer,DGroupID,DGroupDisPer,@CatIDsNames as Categories  " +
            "		 ,U.UnitShortName as [Unit],ISNULL(BatchCode,0) as BatchCode,BrandID,ISNULL(AltUnitID,0) as AltUnitID,ISNULL(ConvFactor,0) as ConvFactor,ISNULL(Shelflife,0) as Shelflife,ISNULL(SRateInclusive,0) as SRateInclusive,ISNULL(PRateInclusive,0) as PRateInclusive,ISNULL(Slabsys,0) as Slabsystem,batchMode,ISNULL(DiscPer,0) AS DiscPer,S.BatchUnique, S.StockID,ISNULL(DepartmentID,0) as DepartmentID,ISNULL(CompCessQty,0) as CompCessQty " +
            "		 FROM tblItemMaster I " +
            "		 INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID " +
            "		 LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID " +
            "		 LEFT JOIN tblStock S ON S.ItemID =I.ItemID " +
            "         WHERE I.ItemID = @ItemID AND I.TenantID = @TenantID " +
            "	 END " +
            "     ELSE " +
            "     BEGIN " +
            "         SELECT I.ItemID,ItemCode as [Item Code],ItemName as [Item],U.UnitShortName as [Unit],C.Category,Description,I.MRP,HSNID as [HSN Code],(CASE WHEN ActiveStatus = 1 THEN 'Active' ELSE 'In Active' END) as Status, " +
            "		 ISNULL(BatchCode,0) as BatchCode " +
            "		 FROM tblItemMaster I  " +
            "		 INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID " +
            "		 LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID " +
            "		 LEFT JOIN tblStock S ON S.ItemID = I.ItemID " +
            "         WHERE I.TenantID = @TenantID " +
            "     END " +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblPurchase ALTER COLUMN ReferenceAutoNO VARCHAR(50)";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseInsert')" +
             "DROP PROCEDURE UspPurchaseInsert";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspPurchaseInsert] " +
             "( " +
             "     @InvId    NUMERIC  (18,0)," +
             "     @InvNo    VARCHAR  (100)," +
             "     @AutoNum    NUMERIC  (18,0)," +
             "     @Prefix    VARCHAR  (50)," +
             "     @InvDate    DATETIME," +
             "     @VchType    VARCHAR  (100)," +
             "     @MOP    VARCHAR  (100)," +
             "     @TaxModeID    NUMERIC  (18,0)," +
             "     @LedgerId    NUMERIC  (18,0)," +
             "     @Party    VARCHAR  (100)," +
             "     @Discount    FLOAT," +
             "     @TaxAmt    FLOAT," +
             "     @GrossAmt    FLOAT," +
             "     @BillAmt    FLOAT," +
             "     @Cancelled    NUMERIC  (18,0)," +
             "     @OtherExpense    FLOAT," +
             "     @SalesManID    NUMERIC  (18,0)," +
             "     @Taxable    FLOAT," +
             "     @NonTaxable    FLOAT," +
             "     @ItemDiscountTotal    FLOAT," +
             "     @RoundOff    FLOAT," +
             "     @UserNarration    VARCHAR  (500)," +
             "     @SortNumber    NUMERIC  (18,0)," +
             "     @DiscPer    FLOAT," +
             "     @VchTypeID    NUMERIC  (18,0)," +
             "     @CCID    NUMERIC  (18,0)," +
             "     @CurrencyID    NUMERIC  (18,0)," +
             "     @PartyAddress    VARCHAR  (500)," +
             "     @UserID    INT," +
             "     @AgentID    NUMERIC  (18,0)," +
             "     @CashDiscount    FLOAT," +
             "     @DPerType_ManualCalc_Customer    NUMERIC  (18,0)," +
             "     @NetAmount    FLOAT," +
             "     @RefNo    VARCHAR  (100)," +
             "     @CashPaid    NUMERIC  (18,0)," +
             "     @CardPaid    NUMERIC  (18,0)," +
             "     @blnWaitforAuthorisation    NUMERIC  (18,0)," +
             "     @UserIDAuth    NUMERIC  (18,0)," +
             "     @BillTime    DATETIME," +
             "     @StateID    NUMERIC  (18,0)," +
             "     @ImplementingStateCode    VARCHAR  (50)," +
             "     @GSTType    VARCHAR  (50)," +
             "     @CGSTTotal    FLOAT," +
             "     @SGSTTotal    FLOAT," +
             "     @IGSTTotal    FLOAT," +
             "     @PartyGSTIN    VARCHAR  (50)," +
             "     @BillType    VARCHAR  (50)," +
             "     @blnHold    NUMERIC  (18,0)," +
             "     @PriceListID    NUMERIC  (18,0)," +
             "     @EffectiveDate    DATETIME," +
             "     @partyCode    VARCHAR  (150)," +
             "     @MobileNo    VARCHAR  (20)," +
             "     @Email    VARCHAR  (100)," +
             "     @TaxType    VARCHAR  (50)," +
             "     @QtyTotal    FLOAT," +
             "     @DestCCID    NUMERIC  (18,0)," +
             "     @AgentCommMode    VARCHAR  (50)," +
             "     @AgentCommAmount    FLOAT," +
             "     @AgentLID    NUMERIC  (18,0)," +
             "     @BlnStockInsert    NUMERIC  (18,0)," +
             "     @BlnConverted    NUMERIC  (18,0)," +
             "     @ConvertedParentVchTypeID    NUMERIC  (18,0)," +
             "     @ConvertedVchTypeID    NUMERIC  (18,0)," +
             "     @ConvertedVchNo    VARCHAR  (50)," +
             "     @ConvertedVchID    NUMERIC  (18,0)," +
             "     @DeliveryNoteDetails    VARCHAR  (500)," +
             "     @OrderDetails    VARCHAR  (500)," +
             "     @IntegrityStatus    VARCHAR  (50)," +
             "     @BalQty    FLOAT," +
             "     @CustomerpointsSettled    FLOAT," +
             "     @blnCashPaid    NUMERIC  (18,0)," +
             "     @originalsalesinvid    NUMERIC  (18,0)," +
             "     @retuninvid    NUMERIC  (18,0)," +
             "     @returnamount    FLOAT," +
             "     @SystemName    VARCHAR  (50)," +
             "     @LastUpdateDate    DATETIME," +
             "     @LastUpdateTime    DATETIME," +
             "     @DeliveryDetails    VARCHAR  (MAX)," +
             "     @DespatchDetails    VARCHAR  (MAX)," +
             "     @TermsOfDelivery    VARCHAR  (MAX)," +
             "     @FloodCessTot    FLOAT," +
             "     @CounterID    NUMERIC  (18,0)," +
             "     @ExtraCharges    FLOAT," +
             "     @ReferenceAutoNO   VARCHAR(50)," +
             "	 @CashDiscPer FLOAT," +
             "	 @CostFactor numeric(18, 0)," +
             "     @TenantID    NUMERIC  (18,0)," +
             "     @JsonData   VARCHAR  (MAX)," +
             "	 @Action             INT=0" +
             ")" +
             "AS " +
             " BEGIN " +
             " DECLARE @RetResult      INT " +
             " BEGIN TRY " +
             " BEGIN TRANSACTION; " +
             "IF @Action = 0 " +
             " BEGIN " +
             "     INSERT INTO tblPurchase(InvId,InvNo,AutoNum,Prefix,InvDate,VchType,MOP,TaxModeID,LedgerId,Party,Discount,TaxAmt,GrossAmt,BillAmt,Cancelled,OtherExpense,SalesManID,Taxable,NonTaxable,ItemDiscountTotal,RoundOff,UserNarration,SortNumber,DiscPer,VchTypeID,CCID,CurrencyID,PartyAddress,UserID,AgentID,CashDiscount,DPerType_ManualCalc_Customer,NetAmount,RefNo,CashPaid,CardPaid,blnWaitforAuthorisation,UserIDAuth,BillTime,StateID,ImplementingStateCode,GSTType,CGSTTotal,SGSTTotal,IGSTTotal,PartyGSTIN,BillType,blnHold,PriceListID,EffectiveDate,partyCode,MobileNo,Email,TaxType,QtyTotal,DestCCID,AgentCommMode,AgentCommAmount,AgentLID,BlnStockInsert,BlnConverted,ConvertedParentVchTypeID,ConvertedVchTypeID,ConvertedVchNo,ConvertedVchID,DeliveryNoteDetails,OrderDetails,IntegrityStatus,BalQty,CustomerpointsSettled,blnCashPaid,originalsalesinvid,retuninvid,returnamount,SystemName,LastUpdateDate,LastUpdateTime,DeliveryDetails,DespatchDetails,TermsOfDelivery,FloodCessTot,CounterID,ExtraCharges,ReferenceAutoNO,CashDisPer,CostFactor,TenantID,JsonData)" +
             "     VALUES(@InvId,@InvNo,@AutoNum,@Prefix,@InvDate,@VchType,@MOP,@TaxModeID,@LedgerId,@Party,@Discount,@TaxAmt,@GrossAmt,@BillAmt,@Cancelled,@OtherExpense,@SalesManID,@Taxable,@NonTaxable,@ItemDiscountTotal,@RoundOff,@UserNarration,@SortNumber,@DiscPer,@VchTypeID,@CCID,@CurrencyID,@PartyAddress,@UserID,@AgentID,@CashDiscount,@DPerType_ManualCalc_Customer,@NetAmount,@RefNo,@CashPaid,@CardPaid,@blnWaitforAuthorisation,@UserIDAuth,@BillTime,@StateID,@ImplementingStateCode,@GSTType,@CGSTTotal,@SGSTTotal,@IGSTTotal,@PartyGSTIN,@BillType,@blnHold,@PriceListID,@EffectiveDate,@partyCode,@MobileNo,@Email,@TaxType,@QtyTotal,@DestCCID,@AgentCommMode,@AgentCommAmount,@AgentLID,@BlnStockInsert,@BlnConverted,@ConvertedParentVchTypeID,@ConvertedVchTypeID,@ConvertedVchNo,@ConvertedVchID,@DeliveryNoteDetails,@OrderDetails,@IntegrityStatus,@BalQty,@CustomerpointsSettled,@blnCashPaid,@originalsalesinvid,@retuninvid,@returnamount,@SystemName,@LastUpdateDate,@LastUpdateTime,@DeliveryDetails,@DespatchDetails,@TermsOfDelivery,@FloodCessTot,@CounterID,@ExtraCharges,@ReferenceAutoNO,@CashDiscPer,@CostFactor,@TenantID,@JsonData)" +
             "     SET @RetResult = 1;" +
             " END " +
             "IF @Action = 1 " +
             " BEGIN " +
             "     UPDATE tblPurchase SET InvNo=@InvNo,AutoNum=@AutoNum,Prefix=@Prefix,InvDate=@InvDate,VchType=@VchType,MOP=@MOP,TaxModeID=@TaxModeID,LedgerId=@LedgerId,Party=@Party,Discount=@Discount,TaxAmt=@TaxAmt,GrossAmt=@GrossAmt,BillAmt=@BillAmt,Cancelled=@Cancelled,OtherExpense=@OtherExpense,SalesManID=@SalesManID,Taxable=@Taxable,NonTaxable=@NonTaxable,ItemDiscountTotal=@ItemDiscountTotal,RoundOff=@RoundOff,UserNarration=@UserNarration,SortNumber=@SortNumber,DiscPer=@DiscPer,VchTypeID=@VchTypeID,CCID=@CCID,CurrencyID=@CurrencyID,PartyAddress=@PartyAddress,UserID=@UserID,AgentID=@AgentID,CashDiscount=@CashDiscount,DPerType_ManualCalc_Customer=@DPerType_ManualCalc_Customer,NetAmount=@NetAmount,RefNo=@RefNo,CashPaid=@CashPaid,CardPaid=@CardPaid,blnWaitforAuthorisation=@blnWaitforAuthorisation,UserIDAuth=@UserIDAuth,BillTime=@BillTime,StateID=@StateID,ImplementingStateCode=@ImplementingStateCode,GSTType=@GSTType,CGSTTotal=@CGSTTotal,SGSTTotal=@SGSTTotal,IGSTTotal=@IGSTTotal,PartyGSTIN=@PartyGSTIN,BillType=@BillType,blnHold=@blnHold,PriceListID=@PriceListID,EffectiveDate=@EffectiveDate,partyCode=@partyCode,MobileNo=@MobileNo,Email=@Email,TaxType=@TaxType,QtyTotal=@QtyTotal,DestCCID=@DestCCID,AgentCommMode=@AgentCommMode,AgentCommAmount=@AgentCommAmount,AgentLID=@AgentLID,BlnStockInsert=@BlnStockInsert,BlnConverted=@BlnConverted,ConvertedParentVchTypeID=@ConvertedParentVchTypeID,ConvertedVchTypeID=@ConvertedVchTypeID,ConvertedVchNo=@ConvertedVchNo,ConvertedVchID=@ConvertedVchID,DeliveryNoteDetails=@DeliveryNoteDetails,OrderDetails=@OrderDetails,IntegrityStatus=@IntegrityStatus,BalQty=@BalQty,CustomerpointsSettled=@CustomerpointsSettled,blnCashPaid=@blnCashPaid,originalsalesinvid=@originalsalesinvid,retuninvid=@retuninvid,returnamount=@returnamount,SystemName=@SystemName,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime,DeliveryDetails=@DeliveryDetails,DespatchDetails=@DespatchDetails,TermsOfDelivery=@TermsOfDelivery,FloodCessTot=@FloodCessTot,CounterID=@CounterID,ExtraCharges=@ExtraCharges,ReferenceAutoNO=@ReferenceAutoNO," +
             "	 CashDisPer=@CashDiscPer,CostFactor=@CostFactor,JsonData=@JsonData " +
             "     WHERE InvId=@InvId AND TenantID=@TenantID " +
             "     SET @RetResult = 1; " +
             " END " +
             "IF @Action = 2 " +
             " BEGIN " +
             "     DELETE FROM tblPurchase WHERE InvId=@InvId AND TenantID=@TenantID " +
             "     SET @RetResult = 0; " +
             " END " +
             "IF @Action = 3 " +
             " BEGIN " +
             "     UPDATE tblPurchase SET Cancelled = 1 WHERE InvId=@InvId AND TenantID=@TenantID " +
             "     SET @RetResult = 3; " +
             " END " +
             "COMMIT TRANSACTION; " +
             "SELECT @RetResult as SqlSpResult " +
             " END TRY " +
             " BEGIN CATCH " +
             "ROLLBACK; " +
             "SELECT " +
             "- 1 as SqlSpResult," +
             "ERROR_NUMBER() AS ErrorNumber, " +
             "ERROR_STATE() AS ErrorState, " +
             "ERROR_SEVERITY() AS ErrorSeverity, " +
             "ERROR_PROCEDURE() AS ErrorProcedure, " +
             "ERROR_LINE() AS ErrorLine, " +
             "ERROR_MESSAGE() AS ErrorMessage; " +
             " END CATCH; " +
             " END " +
             "";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetHSNFromItemMaster') " +
           "DROP PROCEDURE UspGetHSNFromItemMaster ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetHSNFromItemMaster] " +
           "( " +
           "@ItemID			NUMERIC(18,0), " +
           "@TenantID		NUMERIC(18,0), " +
           "@HSNID			NUMERIC(18,0) " +
           ") " +
           "AS " +
           " BEGIN " +
           "IF @ItemID <> 0 " +
           " BEGIN " +
           "SELECT Distinct HSNID,IGSTTaxPer as IGSTTaxPer,SGSTTaxPer as SGSTTaxPer,CGSTTaxPer as CGSTTaxPer, CessPer " +
           "FROM tblItemMaster I WHERE I.TenantID = @TenantID AND I.ItemID = @ItemID AND ActiveStatus = 1 " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "IF @HSNID <> 0 " +
           " BEGIN " +
           "SELECT Distinct HSNID,IGSTTaxPer,CessPer,CGSTTaxPer,SGSTTaxPer " +
           "FROM tblItemMaster I WHERE I.TenantID = @TenantID AND ActiveStatus = 1 AND I.HSNID = @HSNID " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "SELECT Distinct HSNID as [HSN Code],IGSTTaxPer as [IGST %],CessPer as [Cess %] " +
           "FROM tblItemMaster I WHERE I.TenantID = @TenantID AND ActiveStatus = 1 " +
           " END " +
           " END " +
           " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetItmMasterWhole') " +
           "DROP PROCEDURE UspGetItmMasterWhole ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetItmMasterWhole] " +
           "( " +
           "@ItemID			NUMERIC(18,0), " +
           "@TenantID		NUMERIC(18,0) " +
           ") " +
           "AS " +
           " BEGIN " +
           "IF @ItemID <> 0 " +
           " BEGIN " +
           "SELECT I.ItemID,ItemCode,ItemName,I.CategoryID,Description,PRate,ISNULL(SrateCalcMode,0) as SrateCalcMode,CRateAvg,Srate1Per,I.SRate1,Srate2Per,I.SRate2,Srate3Per,I.SRate3,I.Srate4,Srate4Per,I.SRate5,Srate5Per,I.MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,I.UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,PGID,ItemCodeUniCode,UPC,BatchMode,blnExpiry,Qty,MaxQty,IntNoOrWeight,I.SystemName,I.UserID,I.LastUpdateDate,I.LastUpdateTime,I.TenantID,blnCessOnTax,CompCessQty,CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,CessPer,VAT,CategoryIDs,ColorIDs,SizeIDs,BrandDisPer,DGroupID,DGroupDisPer " +
           ",U.UnitShortName as [Unit],BrandID,ISNULL(AltUnitID,0) as AltUnitID,ISNULL(ConvFactor,0) as ConvFactor,ISNULL(Shelflife,0) as Shelflife,ISNULL(SRateInclusive,0) as SRateInclusive,ISNULL(PRateInclusive,0) as PRateInclusive,ISNULL(Slabsys,0) as Slabsystem " +
           "FROM tblItemMaster I " +
           "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID " +
           "LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID " +
           "WHERE I.ItemID = @ItemID AND I.TenantID = @TenantID " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "SELECT I.ItemID,ItemCode as [Item Code],ItemName as [Item],U.UnitShortName as [Unit],C.Category,Description,I.MRP,HSNID as [HSN Code],(CASE WHEN ActiveStatus = 1 THEN 'Active' ELSE 'In Active' END) as Status, " +
           "IGSTTaxPer as [Tax %] " +
           "FROM tblItemMaster I " +
           "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID " +
           "LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID " +
           "WHERE I.TenantID = @TenantID " +
           " END " +
           " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetLedgerDetail') " +
           "DROP PROCEDURE UspGetLedgerDetail ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetLedgerDetail] " +
           "( " +
           "@LID DECIMAL, " +
           "@TenantID DECIMAL, " +
           "@AccGpID DECIMAL=NULL " +
           ") " +
           "AS " +
           " BEGIN " +
           "IF @LID <> 0 " +
           " BEGIN " +
           "SELECT LID,LName,LAliasName,GroupName,Type,OpBalance,Address,CreditDays,Phone,AccountGroupID,Area,Email,MobileNo,DiscPer,AreaID,ActiveStatus,EmailAddress,SystemName,UserID,LastUpdateDate,LastUpdateTime,TaxNo,GSTType,ISNULL(StateID,0) as StateID ,(SELECT ISNULL(State,'') FROM tblStates WHERE StateId = L.StateID) As State,(SELECT AccountGroup FROM tblAccountGroup where AccountGroupID=L.AccountGroupID) As AccountGroup " +
           ",AreaID,AgentID,(Select AgentName From tblAgent TA where TA.AgentID = L.AgentID) As AgentName,ISNULL(DiscPer,0) as DiscPer,ISNULL(PLID,0) as PLID " +
           "FROM tblLedger L " +
           "WHERE LID = @LID " +
           "ORDER BY LID ASC " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "IF @AccGpID = 10 " +
           " BEGIN " +
           "SELECT LID,LName As [Customer Name],LAliasName As [AliasName],(SELECT AccountGroup FROM tblAccountGroup where AccountGroupID=L.AccountGroupID) As [AccountGroup],GSTType As [GST Type],Address as [Address],MobileNo as [MobileNo],Email as [Email],(SELECT ISNULL(State,'') FROM tblStates WHERE StateId = L.StateID) As [State] " +
           "FROM tblLedger L WHERE TenantID = @TenantID AND AccountGroupID=10 " +
           "ORDER BY LID ASC " +
           " END " +
           " ELSE IF @AccGpID = 11 " +
           " BEGIN " +
           "SELECT LID,LName As [Supplier Name],LAliasName As [AliasName],(SELECT AccountGroup FROM tblAccountGroup where AccountGroupID=L.AccountGroupID) As [AccountGroup],GSTType As [GST Type],Address as [Address],MobileNo as [MobileNo],Email as [Email],(SELECT ISNULL(State,'') FROM tblStates WHERE StateId = L.StateID) As [State] " +
           "FROM tblLedger L WHERE TenantID = @TenantID AND AccountGroupID=11 " +
           "ORDER BY LID ASC " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "SELECT LID,LName As [Ledger Name],LAliasName As [AliasName],(SELECT AccountGroup FROM tblAccountGroup where AccountGroupID=L.AccountGroupID) As [AccountGroup],GSTType As [GST Type],Address as [Address],MobileNo as [MobileNo],Email as [Email],(SELECT ISNULL(State,'') FROM tblStates WHERE StateId = L.StateID) As [State],TaxNo As [Tax No],OpBalance As [Opening Balance],Type As [Opening Type] " +
           "FROM tblLedger L WHERE TenantID = @TenantID AND(AccountGroupID <> 10 AND  AccountGroupID <> 11) " +
           "ORDER BY LID ASC " +
           " END " +
           " END " +
           " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            //sQuery = "DELETE from tblUserGroupMaster where ID = 4";
            //Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Allow user to Edit MRP'  WHERE  SettingsName = 'BLNEDITMRPRATE'	AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Allow user to Edit Rate'  WHERE  SettingsName = 'blneditsalerate' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Allow user to Edit Tax Percentage'  WHERE  SettingsName = 'blnEditTaxPer' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'View Cash Discount'  WHERE  SettingsName = 'blnenablecashdiscount' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'View Effective Date'  WHERE  SettingsName = 'blnenableEffeciveDate' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'View Party Details'  WHERE  SettingsName = 'blnpartydetails' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Ask Confirmation on Print'  WHERE  SettingsName = 'blnprintconfirmation' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Send Bill to Printer on Save'  WHERE  SettingsName = 'blnprintimmediately' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'RECALCULATE SalesRates On Percentage'  WHERE  SettingsName = 'BLNRECALCULATESalesRatesOnPercentage' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Enable Bill Narration'  WHERE  SettingsName = 'blnshowbillnarration' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Enable Free Qty'  WHERE  SettingsName = 'BLNSHOWFREEQUANTITY' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET BlnEnabled = 0  WHERE  SettingsName = 'blnShowItemCalcGrid' AND VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Show Product Profit Percentage'  WHERE  SettingsName = 'blnShowItemProfitPer' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Enable Other Expenses'  WHERE  SettingsName = 'blnshowotherexpense' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Show Preview before Print'  WHERE  SettingsName = 'blnshowpreview' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Enable Rate Fixer'  WHERE  SettingsName = 'blnShowRateFixer' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Enable Reference No'  WHERE  SettingsName = 'blnShowReferenceNo' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) " +
            "UPDATE tblvchtypeGenSettings SET BlnEnabled = 0  WHERE  SettingsName = 'blnSummariseDuplicateItemsInPrint' AND VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2) " +
            "UPDATE tblvchtypeGenSettings SET BlnEnabled = 0  WHERE  SettingsName = 'blnSummariseItemsWhileEntering' AND VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2) " +
            "UPDATE tblvchtypeGenSettings SET SettingsDescription = 'Validate Sales Rate is less than Purchase Rate'  WHERE  SettingsName = 'blnWarnifSRatelessthanPrate' AND (VchTypeID IN (select VchTypeID from tblVchType Where ParentID = 2)) ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetBatchCodeWhenAutoBarcode') " +
            "DROP PROCEDURE UspGetBatchCodeWhenAutoBarcode)" +
            "GO";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery =
            " " +
            "CREATE PROCEDURE [dbo].[UspGetBatchCodeWhenAutoBarcode]" +
            "(" +
            "	@ItemID			NUMERIC(18,0)," +
            "	@BatchCode		VARCHAR(50)," +
            "	@BatchUniq		VARCHAR(50)," +
            "	@MRP			NUMERIC(18,5)," +
            "	@ExpDt			DATETIME," +
            "	@TenantID		NUMERIC(18,0)" +
            ") " +
            "AS " +
            " BEGIN " +
            "	DECLARE @BatchID		NUMERIC(18,0)" +
            "	DECLARE @BLNADVANCED	INT" +
            "	DECLARE @blnExpiry		BIT" +
            "	SELECT @BatchID = ISNULL(MAX(BatchID) + 1,0) FROM tblStock WHERE TenantID = @TenantID" +
            "	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED' " +
            "	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID" +
            "	IF @BatchID = 0" +
            "	BEGIN " +
            "		SET @BatchID = 1" +
            "	END " +
            "	IF @BatchCode = '<Auto Barcode>'" +
            "	BEGIN " +
            "		Declare @Prefix VARCHAR(50)" +
            "		Declare @BatchPrefix VARCHAR(50)" +
            "		IF @BLNADVANCED = 1" +
            "		BEGIN " +
            "			Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'" +
            "			set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))" +
            "			IF(@BatchPrefix='<YEARMONTH>')" +
            "			BEGIN " +
            "				SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))" +
            "				SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)" +
            "			END " +
            "			ELSE " +
            "			BEGIN " +
            "				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'" +
            "				set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))" +
            "				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID" +
            "				SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)" +
            "			END " +
            "		END " +
            "		ELSE " +
            "		BEGIN " +
            "			SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID" +
            "		END " +
            "		IF @blnExpiry = 1" +
            "		BEGIN " +
            "			SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')" +
            "		END " +
            "		ELSE " +
            "		BEGIN " +
            "			SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) " +
            "		END " +
            "	END " +
            "	SELECT @BatchUniq as 'BatchUniq', @BatchCode as 'BatchCode'" +
            " END " +
            "";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM tblDepartment WHERE DepartmentID= 1) " +
           "UPDATE tblDepartment SET Department = 'DEFAULT DEP', Description = 'Default' WHERE DepartmentID = 1 " +
           " ELSE " +
           " BEGIN " +
           "INSERT INTO tblDepartment(DepartmentID, Department, Description, SystemName, UserID, LastUpdateDate, LastUpdateTime, TenantID) " +
           "VALUES(1, 'DEFAULT DEP', 'Default', '', 1, GETDATE(), GETDATE(), 1) " +
           " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockHistory') " +
           "DROP PROCEDURE UspGetStockHistory ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetStockHistory] " +
             "( " +
            "@ItemID Numeric(18,0), " +
            "@VchTypeID Numeric(18,0)= NULL, " +
            "@BatchUnique varchar(MAX)= NULL, " +
            "@CostCentreID Numeric(18,0)= NULL, " +
            "@FromDate Datetime, " +
            "@ToDate Datetime, " +
            "@TenantID Numeric(18,0) " +
            ") " +
            "AS " +
            " BEGIN " +
            " DECLARE @TempTable Table " +
            "( " +
            "VoucherType varchar(500), " +
            "InvoiceNo Numeric(18, 0), " +
            "VoucherDate datetime, " +
            "Batch varchar(500), " +
            "QtyIn Numeric(18, 0), " +
            "QtyOut Numeric(18, 0), " +
            "PRate Numeric(18, 4), " +
            "SRate Numeric(18, 4), " +
            "Vchtypeid Numeric(18, 0), " +
            "StockID Numeric(18, 0), " +
            "Unit varchar(500), " +
            "ItemID Numeric(18, 0), " +
            "ItemCode varchar(500), " +
            "CCID Numeric(18, 0) " +
            ") " +
            "INSERT INTO @TempTable " +
            "SELECT " +
            "TSH.vchtype , " +
            "invid , " +
            "VchDate, " +
            "TSH.batchUnique , " +
            "round(isnull(QTYIN, 0), 3), " +
            "round(isnull(QTYOUT, 0), 3), " +
            "Round(TSH.PRateExcl, 2) , " +
            "Round(TSH.SRate1, 2) , " +
            "VIA.Vchtypeid, " +
            "TS.StockID, " +
            "TU.UnitShortName,  " +
            "TSH.ItemID, " +
            "ItemCode, " +
            "TSH.CCID " +
            "FROM tblStockHistory TSH " +
            "LEFT JOIN tblstock TS ON TSH.BatchUnique = TS.batchUnique " +
            "LEFT JOIN VWitemAnalysis VIA ON TSH.RefId = VIA.Invid " +
            "LEFT JOIN tblItemMaster IM ON TSH.ItemID = IM.ItemID " +
            "LEFT JOIN tblUnit TU ON IM.UNITID = TU.UnitID " +
            "WHERE " +
            "TSH.TenantID = @TenantID " +
            "AND TSH.ItemID = @ItemID " +
            "AND convert(varchar, VchDate, 106) >= @FromDate " +
            "AND convert(varchar, VchDate, 106) <= @ToDate " +
            "AND VIA.VchType IS NOT NULL " +
            "IF @VchTypeID<> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE VchTypeID<> @VchTypeID " +
            " END " +
            "IF @BatchUnique <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE StockID<> @BatchUnique " +
            " END " +
            "IF @CostCentreID <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE CCID<> @CostCentreID " +
            " END " +
            "SELECT " +
            "VoucherType AS[Voucher Type], " +
            "InvoiceNo AS[Invoice No], " +
            "CONVERT(VARCHAR(12), FORMAT(VoucherDate, 'dd-MMM-yyyy')) As[Voucher Date], " +
            "Batch AS[Batch], " +
            "QtyIn AS[Qty In], " +
            "QtyOut AS[Qty Out], " +
            "Unit AS[Unit], " +
            "PRate AS[P.Rate], " +
            "SRate AS[S.Rate] " +
            "FROM @TempTable " +
            "Order by VoucherDate " +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockHistoryTotal') " +
           "DROP PROCEDURE UspGetStockHistoryTotal";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE[dbo].[UspGetStockHistoryTotal] " +
            "( " +
            "@ItemID Numeric(18,0), " +
            "@VchTypeID Numeric(18,0)= NULL, " +
            "@BatchUnique varchar(MAX)= NULL,  " +
            "@CostCentreID Numeric(18,0)= NULL, " +
            "@FromDate Datetime, " +
            "@ToDate Datetime, " +
            "@TenantID Numeric(18,0) " +
            ") " +
            "AS " +
            " BEGIN " +
            " DECLARE @TempTable Table " +
            "( " +
            "VoucherType varchar(500), " +
            "InvoiceNo Numeric(18, 0), " +
            "VoucherDate datetime, " +
            "Batch varchar(500), " +
            "QtyIn Numeric(18, 0), " +
            "QtyOut Numeric(18, 0), " +
            "PRate Numeric(18, 4), " +
            "SRate Numeric(18, 4), " +
            "Vchtypeid Numeric(18, 0), " +
            "StockID Numeric(18, 0), " +
            "Unit varchar(500), " +
            "ItemID Numeric(18, 0), " +
            "ItemCode varchar(500), " +
            "CCID Numeric(18, 0) " +
            ") " +
            "INSERT INTO @TempTable " +
            "SELECT " +
            "TSH.vchtype , " +
            "invid , " +
            "VchDate, " +
            "TSH.batchUnique ,  " +
            "round(isnull(QTYIN, 0), 3), " +
            "round(isnull(QTYOUT, 0), 3), " +
            "Round(TSH.PRateExcl, 2) , " +
            "Round(TSH.SRate1, 2) , " +
            "TSH.Vchtypeid, " +
            "TS.StockID, " +
            "TU.UnitShortName,  " +
            "TSH.ItemID, " +
            "ItemCode, " +
            "TSH.CCID " +
            "FROM tblStockHistory TSH " +
            "LEFT JOIN tblstock TS ON TSH.BatchUnique = TS.batchUnique " +
            "LEFT JOIN VWitemAnalysis VIA ON TSH.RefId = VIA.Invid " +
            "LEFT JOIN tblItemMaster IM ON TSH.ItemID = IM.ItemID " +
            "LEFT JOIN tblUnit TU ON IM.UNITID = TU.UnitID " +
            "WHERE " +
            "TSH.TenantID = @TenantID " +
            "AND TSH.ItemID = @ItemID " +
            "AND convert(varchar, VchDate, 106) >= @FromDate " +
            "AND convert(varchar, VchDate, 106) <= @ToDate " +
            "AND VIA.VchType IS NOT NULL " +
            "IF @VchTypeID<> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE VchTypeID<> @VchTypeID " +
            " END " +
            "IF @BatchUnique <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE StockID<> @BatchUnique " +
            " END " +
            "IF @CostCentreID <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE CCID<> @CostCentreID " +
            " END " +
            " DECLARE @StockTotalTable Table " +
            "( " +
            "QTYIN Numeric(18,4), " +
            "QTYout Numeric(18,4), " +
            "BalanceQty Numeric(18,4) " +
            ") " +
            "Select " +
            "Sum(isnull(QtyIn, 0)) as TotalQTYIn , " +
            "sum(isnull(QtyOut, 0)) as TotalQTYOut ,  " +
            "(Sum(isnull(QtyIn, 0)) - sum(isnull(QtyOut, 0))) as BalanceQty " +
            "FROM " +
            "@TempTable " +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery =
            "UPDATE tblLedger SET GroupName = 'SUPPLIER', AccountGroupID = 11 WHERE LID = 100 " +
            "UPDATE tblLedger SET GroupName = 'CUSTOMER', AccountGroupID = 10 WHERE LID = 101 ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.sysobjects WHERE xtype = 'P' AND name = 'UspTransStockUpdate')" +
            " BEGIN " +
            " DROP PROCEDURE UspTransStockUpdate" +
            " END " +
            "GO";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspTransStockUpdate]" +
            "(" +
            "	@ItemID			NUMERIC(18,0)," +
            "	@BatchCode		VARCHAR(50)," +
            "	@BatchUniq		VARCHAR(50)," +
            "	@Qty			NUMERIC(18,5)," +
            "	@MRP			NUMERIC(18,5)," +
            "	@CostRateInc	NUMERIC(18,5)," +
            "	@CostRateExcl	NUMERIC(18,5)," +
            "	@PRateExcl		NUMERIC(18,5)," +
            "	@PrateInc		NUMERIC(18,5)," +
            "	@TaxPer			NUMERIC(18,5)," +
            "	@SRate1			NUMERIC(18,5)," +
            "	@SRate2			NUMERIC(18,5)," +
            "	@SRate3			NUMERIC(18,5)," +
            "	@SRate4			NUMERIC(18,5)," +
            "	@SRate5			NUMERIC(18,5)," +
            "	@BatchMode		INT," +
            "	@VchType		VARCHAR(100)," +
            "	@VchDate		DATETIME," +
            "	@ExpDt			DATETIME," +
            "	@Action	 	    VARCHAR(20)," +
            "	@RefID			NUMERIC(18,0)," +
            "	@VchTypeID		NUMERIC(18,0)," +
            "	@CCID			NUMERIC(18,0)," +
            "	@TenantID		NUMERIC(18,0)," +
            "	@BarCode_out	VARCHAR(50) OUTPUT" +
            ")" +
            "AS" +
            " BEGIN " +
            "	DECLARE @BatchID		NUMERIC(18,0)" +
            "	DECLARE @StockID		NUMERIC(18,0)" +
            "	DECLARE @LastInvDt		DATETIME = Getdate()" +
            "	DECLARE @STOCKHISID		NUMERIC(18,0)" +
            "	DECLARE @PRFXBATCH		VARCHAR(10)" +
            "	DECLARE @Stock		   NUMERIC(18,5)" +
            "	DECLARE @INVID          NUMERIC(18,0)" +
            "	DECLARE @BarCode		VARCHAR(50)" +
            "	DECLARE @BarUniq		VARCHAR(100)" +
            "	DECLARE @CalcQOH		NUMERIC(18,5)" +
            "	DECLARE @BLNADVANCED	INT" +
            "	DECLARE @blnExpiry		BIT" +
            "	SET @BarCode = @BatchCode" +
            "	SELECT @StockID = MAX(ISNULL(StockID,0)) + 1 FROM tblStock WHERE TenantID = @TenantID" +
            "	SELECT @BatchID = MAX(ISNULL(BatchID,0)) + 1 FROM tblStock WHERE TenantID = @TenantID" +
            "	SELECT @STOCKHISID = MAX(ISNULL(STOCKHISID,0)) + 1 FROM tblStockHistory WHERE TenantID = @TenantID" +
            "	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED' " +
            "	SELECT @Stock = ISNULL(QOH,0)FROM tblStock WHERE ItemID= @ItemID AND BatchCode = @BatchCode AND TenantID = @TenantID" +
            "	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID" +
            "	IF @StockID = 0" +
            "	BEGIN " +
            "		SET @StockID = 1" +
            "	END " +
            "	IF @STOCKHISID = 0" +
            "	BEGIN " +
            "		SET @STOCKHISID = 1" +
            "	END " +
            "	IF @BatchID = 0" +
            "	BEGIN " +
            "		SET @BatchID = 1" +
            "	END " +
            "	IF @Action = 'STOCKADD'" +
            "	BEGIN " +
            "		IF @BatchCode = '<Auto Barcode>'" +
            "		BEGIN " +
            "			Declare @Prefix VARCHAR(50)" +
            "			Declare @BatchPrefix VARCHAR(50)" +
            "			IF @BLNADVANCED = 1" +
            "			BEGIN " +
            "				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'" +
            "				set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))" +
            "				IF(@BatchPrefix='<YEARMONTH>')" +
            "				BEGIN " +
            "					SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))" +
            "					SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)" +
            "				END " +
            "				ELSE " +
            "				BEGIN " +
            "					Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'" +
            "					set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))" +
            "					SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID" +
            "					IF @BatchPrefix <> ''" +
            "					BEGIN " +
            "						SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)" +
            "					END " +
            "				END " +
            "			END " +
            "			ELSE " +
            "			BEGIN " +
            "				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID" +
            "			END " +
            "			SET @BarCode =  @BatchCode" +
            "			IF @blnExpiry = 1" +
            "			BEGIN " +
            "				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')" +
            "				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')" +
            "			END " +
            "			ELSE " +
            "			BEGIN " +
            "				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) " +
            "				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) " +
            "			END " +
            "			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID) " +
            "			BEGIN " +
            "				IF @VchTypeID <> 0" +
            "				BEGIN " +
            "					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode " +
            "					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)" +
            "					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)" +
            "				END " +
            "			END " +
            "			ELSE " +
            "			BEGIN " +
            "				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID  AND CCID = @CCID) " +
            "				BEGIN " +
            "					IF @VchTypeID <> 0" +
            "					BEGIN " +
            "						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode " +
            "						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)" +
            "						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)" +
            "					END " +
            "				END " +
            "				ELSE " +
            "				BEGIN " +
            "					IF @VchTypeID <> 0" +
            "					BEGIN " +
            "						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode " +
            "						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)" +
            "						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)" +
            "					END " +
            "				END " +
            "			END " +
            "		END " +
            "		ELSE " +
            "		BEGIN " +
            "			IF @BatchMode = 0 " +
            "			BEGIN " +
            "				SELECT @BatchCode = ItemCode FROM tblItemMaster WHERE ItemID = @ItemID" +
            "				SET @BatchUniq = @BatchCode" +
            "				SET @BarCode = @BatchCode" +
            "			END " +
            "			ELSE " +
            "			BEGIN " +
            "				IF CHARINDEX('@',@BatchUniq) = 0" +
            "				BEGIN " +
            "					IF @blnExpiry = 1" +
            "					BEGIN " +
            "						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')" +
            "					END " +
            "					ELSE " +
            "					BEGIN " +
            "						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))" +
            "					END " +
            "				END " +
            "				ELSE " +
            "				BEGIN " +
            "					IF @blnExpiry = 1" +
            "					BEGIN " +
            "						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')" +
            "					END " +
            "					ELSE " +
            "					BEGIN " +
            "						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))" +
            "					END " +
            "				END " +
            "			END " +
            "			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID AND MRP=@MRP AND FORMAT(ExpiryDate,'dd-MM-yy') = FORMAT(@ExpDt,'dd-MM-yy')) " +
            "			BEGIN " +
            "				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode " +
            "				IF @VchTypeID <> 0" +
            "				BEGIN " +
            "					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)" +
            "					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)" +
            "				END " +
            "			END " +
            "			ELSE " +
            "			BEGIN " +
            "				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode " +
            "				IF @VchTypeID <> 0" +
            "				BEGIN " +
            "					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)" +
            "					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)" +
            "				END " +
            "			END " +
            "		END " +
            "		SET @BatchCode = @BarCode" +
            "	END " +
            "	IF @Action = 'STOCKLESS'" +
            "	BEGIN " +
            "		SET @Qty = @Qty * -1;" +
            "	END " +
            "	IF @Action = 'STOCKDEL'" +
            "	BEGIN " +
            "		SELECT @CalcQOH = QOH FROM tblStock WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID" +
            "		UPDATE tblStock SET QOH = QOH - @CalcQOH WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID" +
            "		DELETE FROM tblStockHistory WHERE RefId = @RefID AND CCID = @CCID AND TenantID = @TenantID" +
            "	END " +
            "	SET @BarCode_out = @BatchUniq" +
            "	SELECT @BarCode_out" +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetPurchaseMaster') " +
           "DROP PROCEDURE UspGetPurchaseMaster";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetPurchaseMaster]" +
            "(" +
            "	@InvId			NUMERIC   (18,0)," +
            "	@TenantID		NUMERIC   (18,0)," +
            "	@VchTypeID		NUMERIC   (18,0)," +
            "	@blnPrevNext	BIT = 0" +
            ")" +
            " AS " +
            " BEGIN " +
            "	 declare @PrevVoucherNo int" +
            "	 declare @NextVoucherNo int" +
            "	 DECLARE @InvId_Org	INT" +
            "	 IF @InvId <> 0 " +
            "     BEGIN " +
            "		IF @blnPrevNext = 0" +
            "		BEGIN " +
            "			SELECT party,InvId,InvNo,AutoNum,Prefix, convert(varchar(10), InvDate, 105) as InvDate,convert(varchar(10), EffectiveDate, 105) as EffectiveDate,RefNo,ReferenceAutoNO,MOP,TaxModeID,CCID,SalesManID,AgentID,MobileNo,StateID,GSTType,PartyAddress,GrossAmt," +
            "			ItemDiscountTotal,DiscPer,Discount,Taxable,NonTaxable,TaxAmt,OtherExpense,NetAmount,CashDiscount,RoundOff,UserNarration,BillAmt,PartyGSTIN,Isnull(CashDisPer,0) as CashDisPer ,Isnull(CostFactor,0) as CostFactor,LedgerId,Cancelled,JsonData FROM tblPurchase" +
            "			WHERE InvId = @InvId AND TenantID = @TenantID AND VchTypeID = @VchTypeID" +
            "		END " +
            "		ELSE " +
            "		BEGIN " +
            "			SELECT @InvId_Org = InvId FROM tblPurchase WHERE InvNo = @InvId AND TenantID = @TenantID AND VchTypeID = @VchTypeID" +
            "			SELECT TOP 1 @PrevVoucherNo = InvId FROM tblPurchase WHERE InvId < @InvId_Org AND VchTypeID = @VchTypeID ORDER BY InvId DESC" +
            "			SELECT TOP 1 @NextVoucherNo = InvId FROM tblPurchase WHERE InvId > @InvId_Org AND VchTypeID = @VchTypeID ORDER BY InvId ASC" +
            "			SELECT  ISNULL(@PrevVoucherNo,0) As PrevVoucherNo, ISNULL(@NextVoucherNo,0) As NextVoucherNo" +
            "		END " +
            "     END " +
            "     ELSE " +
            "     BEGIN " +
            "         SELECT " +
            "		 InvId,AutoNum as [Invoice No],CONVERT(varchar(12),InvDate) as [Invoice Date]" +
            "		 ,MOP as [Mode Of Payment],Party as [Supplier],ItemDiscountTotal as [Item Discount]" +
            "		 ,CashDiscount as [Cash Discount] ,Taxable as [Taxable],TaxAmt as [Tax]" +
            "		 ,Discount as [Discount],RoundOff as [RoundOff],BillAmt as [Bill Amount]" +
            "		 ,(CASE WHEN ISNULL(Cancelled,0) = 0 THEN 'Active' ELSE 'Cancelled' END) as [Bill Status]" +
            "		 FROM tblPurchase WHERE TenantID = @TenantID AND VchTypeID = @VchTypeID" +
            "         order by InvID asc" +
            "	 END " +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblVchType ADD GridSettingsJson VARCHAR(MAX)";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseItemInsert') " +
           "DROP PROCEDURE UspPurchaseItemInsert";
            Comm.fnExecuteNonQuery(sQuery, false);

                
            sQuery = "CREATE PROCEDURE [dbo].[UspPurchaseItemInsert]" +
            "(" +
            "     @InvID    NUMERIC  (18,0)," +
            "     @ItemId    NUMERIC  (18,0)," +
            "     @Qty    FLOAT," +
            "     @Rate    FLOAT," +
            "     @UnitId    NUMERIC  (18,0)," +
            "     @Batch    VARCHAR  (50)," +
            "     @TaxPer    FLOAT," +
            "     @TaxAmount    FLOAT," +
            "     @Discount    FLOAT," +
            "     @MRP    FLOAT," +
            "     @SlNo    NUMERIC  (18,0)," +
            "     @Prate    FLOAT," +
            "     @Free    FLOAT," +
            "     @SerialNos    VARCHAR  (5000)," +
            "     @ItemDiscount    FLOAT," +
            "     @BatchCode    VARCHAR  (50)," +
            "     @iCessOnTax    FLOAT," +
            "     @blnCessOnTax    NUMERIC  (18,0)," +
            "     @Expiry    DATETIME," +
            "     @ItemDiscountPer    FLOAT," +
            "     @RateInclusive    NUMERIC  (18,0)," +
            "     @ITaxableAmount    FLOAT," +
            "     @INetAmount    FLOAT," +
            "     @CGSTTaxPer    FLOAT," +
            "     @CGSTTaxAmt    FLOAT," +
            "     @SGSTTaxPer    FLOAT," +
            "     @SGSTTaxAmt    FLOAT," +
            "     @IGSTTaxPer    FLOAT," +
            "     @IGSTTaxAmt    FLOAT," +
            "     @iRateDiscPer    FLOAT," +
            "     @iRateDiscount    FLOAT," +
            "     @BatchUnique    VARCHAR  (150)," +
            "     @blnQtyIN    NUMERIC  (18,0)," +
            "     @CRate    FLOAT," +
            "     @Unit    VARCHAR  (50)," +
            "     @ItemStockID    NUMERIC  (18,0)," +
            "     @IcessPercent    FLOAT," +
            "     @IcessAmt    FLOAT," +
            "     @IQtyCompCessPer    FLOAT," +
            "     @IQtyCompCessAmt    FLOAT," +
            "     @StockMRP    FLOAT," +
            "     @BaseCRate    FLOAT," +
            "     @InonTaxableAmount    FLOAT," +
            "     @IAgentCommPercent    FLOAT," +
            "     @BlnDelete    NUMERIC  (18,0)," +
            "     @Id    NUMERIC  (18,0)," +
            "     @StrOfferDetails    VARCHAR  (100)," +
            "     @BlnOfferItem    FLOAT," +
            "     @BalQty    FLOAT," +
            "     @GrossAmount    FLOAT," +
            "     @iFloodCessPer    FLOAT," +
            "     @iFloodCessAmt    FLOAT," +
            "     @Srate1    FLOAT," +
            "     @Srate2    FLOAT," +
            "     @Srate3    FLOAT," +
            "     @Srate4    FLOAT," +
            "     @Srate5    FLOAT," +
            "     @Costrate    FLOAT," +
            "     @CostValue    FLOAT," +
            "     @Profit    FLOAT," +
            "     @ProfitPer    FLOAT," +
            "     @DiscMode    NUMERIC  (18,0)," +
            "     @Srate1Per    FLOAT," +
            "     @Srate2Per    FLOAT," +
            "     @Srate3Per    FLOAT," +
            "     @Srate4Per    FLOAT," +
            "     @Srate5Per   FLOAT," +
            "	 @Action             INT=0" +
            ")" +
            " AS " +
            " BEGIN " +
            " DECLARE @RetResult  INT" +
            " DECLARE @RetID      INT" +
            " DECLARE @VchType	VARCHAR(50)" +
            " DECLARE @VchTypeID	NUMERIC(18,0)" +
            " DECLARE @BatchMode	VARCHAR(50)" +
            " DECLARE @VchDate	DATETIME" +
            " DECLARE @CCID		NUMERIC(18,0)" +
            " DECLARE @TenantID	NUMERIC(18,0)" +
            " DECLARE @BarCode_out VARCHAR(50)" +
            " BEGIN TRY" +
            " BEGIN TRANSACTION;" +
            " SELECT @VchType = VchType,@VchTypeID = VchTypeID,@VchDate=InvDate,@CCID=CCID,@TenantID=TenantID FROM tblPurchase WHERE InvId = @InvID" +
            " SELECT @BatchMode = BatchMode FROM tblItemMaster WHERE ItemID = @ItemId" +
            " IF @Action = 0" +
            " BEGIN " +
            "	EXEC UspTransStockUpdate @ItemId,@BatchCode,@BatchUnique,@Qty,@MRP,@CRate,@CRate,@Prate,@Prate,@TaxPer,@Srate1,@Srate2,@Srate3,@Srate4,@Srate5,@BatchMode,@VchType,@VchDate,@Expiry,'STOCKADD',@InvID,@VchTypeID,@CCID,@TenantID,@BarCode_out output" +
            "	IF CHARINDEX('@',@BarCode_out) > 0" +
            "	BEGIN " +
            "		SET @BatchCode = SUBSTRING(@BarCode_out,1,CHARINDEX('@',@BarCode_out))" +
            "	END " +
            "	ELSE " +
            "	BEGIN " +
            "		SET @BatchCode = @BarCode_out " +
            "	END " +
            "     INSERT INTO tblPurchaseItem(InvID,ItemId,Qty,Rate,UnitId,Batch,TaxPer,TaxAmount,Discount,MRP,SlNo,Prate,Free,SerialNos,ItemDiscount,BatchCode,iCessOnTax,blnCessOnTax,Expiry,ItemDiscountPer,RateInclusive,ITaxableAmount,INetAmount,CGSTTaxPer,CGSTTaxAmt,SGSTTaxPer,SGSTTaxAmt,IGSTTaxPer,IGSTTaxAmt,iRateDiscPer,iRateDiscount,BatchUnique,blnQtyIN,CRate,Unit,ItemStockID,IcessPercent,IcessAmt,IQtyCompCessPer,IQtyCompCessAmt,StockMRP,BaseCRate,InonTaxableAmount,IAgentCommPercent,BlnDelete,StrOfferDetails,BlnOfferItem,BalQty,GrossAmount,iFloodCessPer,iFloodCessAmt,Srate1,Srate2,Srate3,Srate4,Srate5,Costrate,CostValue,Profit,ProfitPer,DiscMode,Srate1Per,Srate2Per,Srate3Per,Srate4Per,Srate5Per)" +
            "     VALUES(@InvID,@ItemId,@Qty,@Rate,@UnitId,@Batch,@TaxPer,@TaxAmount,@Discount,@MRP,@SlNo,@Prate,@Free,@SerialNos,@ItemDiscount,@BatchCode,@iCessOnTax,@blnCessOnTax,@Expiry,@ItemDiscountPer,@RateInclusive,@ITaxableAmount,@INetAmount,@CGSTTaxPer,@CGSTTaxAmt,@SGSTTaxPer,@SGSTTaxAmt,@IGSTTaxPer,@IGSTTaxAmt,@iRateDiscPer,@iRateDiscount,@BarCode_out,@blnQtyIN,@CRate,@Unit,@ItemStockID,@IcessPercent,@IcessAmt,@IQtyCompCessPer,@IQtyCompCessAmt,@StockMRP,@BaseCRate,@InonTaxableAmount,@IAgentCommPercent,@BlnDelete,@StrOfferDetails,@BlnOfferItem,@BalQty,@GrossAmount,@iFloodCessPer,@iFloodCessAmt,@Srate1,@Srate2,@Srate3,@Srate4,@Srate5,@Costrate,@CostValue,@Profit,@ProfitPer,@DiscMode,@Srate1Per,@Srate2Per,@Srate3Per,@Srate4Per,@Srate5Per)" +
            "     SET @RetResult = 1;" +
            " END " +
            " ELSE IF @Action = 2" +
            " BEGIN " +
            "	 EXEC UspTransStockUpdate @ItemId,@BatchCode,@BatchUnique,@Qty,@MRP,@CRate,@CRate,@Prate,@Prate,@TaxPer,@Srate1,@Srate2,@Srate3,@Srate4,@Srate5,@BatchMode,@VchType,@VchDate,@Expiry,'STOCKDEL',@InvID,@VchTypeID,@CCID,@TenantID,@BarCode_out output" +
            "     DELETE FROM tblPurchaseItem WHERE InvID=@InvID" +
            "     SET @RetResult = 0;" +
            " END " +
            " ELSE IF @Action = 3" +
            " BEGIN " +
            "	EXEC UspTransStockUpdate @ItemId,@BatchCode,@BatchUnique,@Qty,@MRP,@CRate,@CRate,@Prate,@Prate,@TaxPer,@Srate1,@Srate2,@Srate3,@Srate4,@Srate5,@BatchMode,@VchType,@VchDate,@Expiry,'STOCKDEL',@InvID,@VchTypeID,@CCID,@TenantID,@BarCode_out output" +
            "    SET @RetResult = 0;" +
            " END " +
            " COMMIT TRANSACTION;" +
            " SELECT @RetResult as SqlSpResult ,@RetID as PID" +
            " END TRY" +
            " BEGIN CATCH" +
            " ROLLBACK;" +
            " SELECT" +
            " - 1 as SqlSpResult,@RetID as PID," +
            " ERROR_NUMBER() AS ErrorNumber," +
            " ERROR_STATE() AS ErrorState," +
            " ERROR_SEVERITY() AS ErrorSeverity," +
            " ERROR_PROCEDURE() AS ErrorProcedure," +
            " ERROR_LINE() AS ErrorLine," +
            " ERROR_MESSAGE() AS ErrorMessage;" +
            " END CATCH;" +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetHSNFromItemMaster') " +
           "DROP PROCEDURE UspGetHSNFromItemMaster ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetHSNFromItemMaster] " +
           "( " +
           "@ItemID			NUMERIC(18,0), " +
           "@TenantID		NUMERIC(18,0), " +
           "@HSNID			NUMERIC(18,0), " +
           "@IGSTTaxPer     NUMERIC(18,0) " +
           ") " +
           "AS " +
           " BEGIN " +
           "IF @ItemID <> 0 " +
           " BEGIN " +
           "SELECT Distinct HSNID,IGSTTaxPer as IGSTTaxPer,SGSTTaxPer as SGSTTaxPer,CGSTTaxPer as CGSTTaxPer, CessPer " +
           "FROM tblItemMaster I WHERE I.TenantID = @TenantID AND I.ItemID = @ItemID AND ActiveStatus = 1 " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "IF @HSNID <> 0 " +
           " BEGIN " +
           "SELECT Distinct HSNID,IGSTTaxPer,CessPer,CGSTTaxPer,SGSTTaxPer " +
           "FROM tblItemMaster I WHERE I.TenantID = @TenantID AND ActiveStatus = 1 AND I.HSNID = @HSNID AND IGSTTaxPer=@IGSTTaxPer " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "SELECT Distinct HSNID as [HSN Code],IGSTTaxPer as [IGST %],CessPer as [Cess %] " +
           "FROM tblItemMaster I WHERE I.TenantID = @TenantID AND ActiveStatus = 1 " +
           " END " +
           " END " +
           " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetItmMasterWhole') " +
           "DROP PROCEDURE UspGetItmMasterWhole ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetItmMasterWhole] " +
           "( " +
           "@ItemID			NUMERIC(18,0), " +
           "@TenantID		NUMERIC(18,0) " +
           ") " +
           "AS " +
           " BEGIN " +
           "IF @ItemID <> 0 " +
           " BEGIN " +
           "SELECT I.ItemID,ItemCode,ItemName,I.CategoryID,Description,PRate,ISNULL(SrateCalcMode,0) as SrateCalcMode,CRateAvg,Srate1Per,I.SRate1,Srate2Per,I.SRate2,Srate3Per,I.SRate3,I.Srate4,Srate4Per,I.SRate5,Srate5Per,I.MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,I.UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,PGID,ItemCodeUniCode,UPC,BatchMode,blnExpiry,Qty,MaxQty,IntNoOrWeight,I.SystemName,I.UserID,I.LastUpdateDate,I.LastUpdateTime,I.TenantID,blnCessOnTax,CompCessQty,CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,CessPer,VAT,CategoryIDs,ColorIDs,SizeIDs,BrandDisPer,DGroupID,DGroupDisPer " +
           ",U.UnitShortName as [Unit],BrandID,ISNULL(AltUnitID,0) as AltUnitID,ISNULL(ConvFactor,0) as ConvFactor,ISNULL(Shelflife,0) as Shelflife,ISNULL(SRateInclusive,0) as SRateInclusive,ISNULL(PRateInclusive,0) as PRateInclusive,ISNULL(Slabsys,0) as Slabsystem " +
           "FROM tblItemMaster I " +
           "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID " +
           "LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID " +
           "WHERE I.ItemID = @ItemID AND I.TenantID = @TenantID " +
           " END " +
           " ELSE " +
           " BEGIN " +
           "SELECT I.ItemID,ItemCode as [Item Code],ItemName as [Item],C.Category,U.UnitShortName as [Unit],HSNID as [HSN Code],IGSTTaxPer as [Tax %],I.MRP,Description,(CASE WHEN ActiveStatus = 1 THEN 'Active' ELSE 'In Active' END) as Status " +
           "FROM tblItemMaster I " +
           "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID " +
           "LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID " +
           "WHERE I.TenantID = @TenantID " +
           " END " +
           " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockHistory') " +
                       "DROP PROCEDURE UspGetStockHistory ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetStockHistory] " +
             "( " +
            "@ItemID Numeric(18,0), " +
            "@VchTypeID Numeric(18,0)= NULL, " +
            "@BatchUnique varchar(MAX)= NULL, " +
            "@CostCentreID Numeric(18,0)= NULL, " +
            "@FromDate Datetime, " +
            "@ToDate Datetime, " +
            "@TenantID Numeric(18,0) " +
            ") " +
            "AS " +
            " BEGIN " +
            " DECLARE @TempTable Table " +
            "( " +
            "VoucherType varchar(500), " +
            "InvoiceNo Numeric(18, 0), " +
            "VoucherDate datetime, " +
            "Batch varchar(500), " +
            "QtyIn Numeric(18, 0), " +
            "QtyOut Numeric(18, 0), " +
            "PRate Numeric(18, 4), " +
            "SRate Numeric(18, 4), " +
            "Vchtypeid Numeric(18, 0), " +
            "StockID Numeric(18, 0), " +
            "Unit varchar(500), " +
            "ItemID Numeric(18, 0), " +
            "ItemCode varchar(500), " +
            "CCID Numeric(18, 0) " +
            ") " +
            "INSERT INTO @TempTable " +
            "SELECT " +
            "TSH.vchtype , " +
            "Invno , " +
            "VchDate, " +
            "TSH.batchUnique , " +
            "round(isnull(QTYIN, 0), 3), " +
            "round(isnull(QTYOUT, 0), 3), " +
            "Round(TSH.PRateExcl, 2) , " +
            "Round(TSH.SRate1, 2) , " +
            "VIA.Vchtypeid, " +
            "TS.StockID, " +
            "TU.UnitShortName,  " +
            "TSH.ItemID, " +
            "ItemCode, " +
            "TSH.CCID " +
            "FROM tblStockHistory TSH " +
            "LEFT JOIN tblstock TS ON TSH.BatchUnique = TS.batchUnique " +
            "LEFT JOIN VWitemAnalysis VIA ON TSH.RefId = VIA.Invid " +
            "LEFT JOIN tblItemMaster IM ON TSH.ItemID = IM.ItemID " +
            "LEFT JOIN tblUnit TU ON IM.UNITID = TU.UnitID " +
            "WHERE " +
            "TSH.TenantID = @TenantID " +
            "AND TSH.ItemID = @ItemID " +
            "AND convert(varchar, VchDate, 106) >= @FromDate " +
            "AND convert(varchar, VchDate, 106) <= @ToDate " +
            "AND VIA.VchType IS NOT NULL " +
            "IF @VchTypeID<> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE VchTypeID<> @VchTypeID " +
            " END " +
            "IF @BatchUnique <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE StockID<> @BatchUnique " +
            " END " +
            "IF @CostCentreID <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE CCID<> @CostCentreID " +
            " END " +
            "SELECT " +
            "VoucherType AS[Voucher Type], " +
            "InvoiceNo AS[Invoice No], " +
            "CONVERT(VARCHAR(12), FORMAT(VoucherDate, 'dd-MMM-yyyy')) As[Voucher Date], " +
            "Batch AS[Batch], " +
            "QtyIn AS[Qty In], " +
            "QtyOut AS[Qty Out], " +
            "Unit AS[Unit], " +
            "PRate AS[P.Rate], " +
            "SRate AS[S.Rate] " +
            "FROM @TempTable " +
            "Order by VoucherDate " +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockHistoryTotal') " +
           "DROP PROCEDURE UspGetStockHistoryTotal";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE[dbo].[UspGetStockHistoryTotal] " +
            "( " +
            "@ItemID Numeric(18,0), " +
            "@VchTypeID Numeric(18,0)= NULL, " +
            "@BatchUnique varchar(MAX)= NULL,  " +
            "@CostCentreID Numeric(18,0)= NULL, " +
            "@FromDate Datetime, " +
            "@ToDate Datetime, " +
            "@TenantID Numeric(18,0) " +
            ") " +
            "AS " +
            " BEGIN " +
            " DECLARE @TempTable Table " +
            "( " +
            "VoucherType varchar(500), " +
            "InvoiceNo Numeric(18, 0), " +
            "VoucherDate datetime, " +
            "Batch varchar(500), " +
            "QtyIn Numeric(18, 0), " +
            "QtyOut Numeric(18, 0), " +
            "PRate Numeric(18, 4), " +
            "SRate Numeric(18, 4), " +
            "Vchtypeid Numeric(18, 0), " +
            "StockID Numeric(18, 0), " +
            "Unit varchar(500), " +
            "ItemID Numeric(18, 0), " +
            "ItemCode varchar(500), " +
            "CCID Numeric(18, 0) " +
            ") " +
            "INSERT INTO @TempTable " +
            "SELECT " +
            "TSH.vchtype , " +
            "Invno , " +
            "VchDate, " +
            "TSH.batchUnique ,  " +
            "round(isnull(QTYIN, 0), 3), " +
            "round(isnull(QTYOUT, 0), 3), " +
            "Round(TSH.PRateExcl, 2) , " +
            "Round(TSH.SRate1, 2) , " +
            "TSH.Vchtypeid, " +
            "TS.StockID, " +
            "TU.UnitShortName,  " +
            "TSH.ItemID, " +
            "ItemCode, " +
            "TSH.CCID " +
            "FROM tblStockHistory TSH " +
            "LEFT JOIN tblstock TS ON TSH.BatchUnique = TS.batchUnique " +
            "LEFT JOIN VWitemAnalysis VIA ON TSH.RefId = VIA.Invid " +
            "LEFT JOIN tblItemMaster IM ON TSH.ItemID = IM.ItemID " +
            "LEFT JOIN tblUnit TU ON IM.UNITID = TU.UnitID " +
            "WHERE " +
            "TSH.TenantID = @TenantID " +
            "AND TSH.ItemID = @ItemID " +
            "AND convert(varchar, VchDate, 106) >= @FromDate " +
            "AND convert(varchar, VchDate, 106) <= @ToDate " +
            "AND VIA.VchType IS NOT NULL " +
            "IF @VchTypeID<> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE VchTypeID<> @VchTypeID " +
            " END " +
            "IF @BatchUnique <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE StockID<> @BatchUnique " +
            " END " +
            "IF @CostCentreID <> 0 " +
            " BEGIN " +
            "DELETE FROM @TempTable WHERE CCID<> @CostCentreID " +
            " END " +
            " DECLARE @StockTotalTable Table " +
            "( " +
            "QTYIN Numeric(18,4), " +
            "QTYout Numeric(18,4), " +
            "BalanceQty Numeric(18,4) " +
            ") " +
            "Select " +
            "Sum(isnull(QtyIn, 0)) as TotalQTYIn , " +
            "sum(isnull(QtyOut, 0)) as TotalQTYOut ,  " +
            "(Sum(isnull(QtyIn, 0)) - sum(isnull(QtyOut, 0))) as BalanceQty " +
            "FROM " +
            "@TempTable " +
            " END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdate') " +
            "DROP PROCEDURE UspTransStockUpdate";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspTransStockUpdate]  " +
                "   (  " +
                "   	@ItemID			NUMERIC(18,0),  " +
                "   	@BatchCode		VARCHAR(50),  " +
                "   	@BatchUniq		VARCHAR(50),  " +
                "   	@Qty			NUMERIC(18,5),  " +
                "   	@MRP			NUMERIC(18,5),  " +
                "   	@CostRateInc	NUMERIC(18,5),  " +
                "   	@CostRateExcl	NUMERIC(18,5),  " +
                "   	@PRateExcl		NUMERIC(18,5),  " +
                "   	@PrateInc		NUMERIC(18,5),  " +
                "   	@TaxPer			NUMERIC(18,5),  " +
                "   	@SRate1			NUMERIC(18,5),  " +
                "   	@SRate2			NUMERIC(18,5),  " +
                "   	@SRate3			NUMERIC(18,5),  " +
                "   	@SRate4			NUMERIC(18,5),  " +
                "   	@SRate5			NUMERIC(18,5),  " +
                "   	@BatchMode		INT,  " +
                "   	@VchType		VARCHAR(100),  " +
                "   	@VchDate		DATETIME,  " +
                "   	@ExpDt			DATETIME,  " +
                "   	@Action	 	    VARCHAR(20),  " +
                "   	@RefID			NUMERIC(18,0),  " +
                "   	@VchTypeID		NUMERIC(18,0),  " +
                "   	@CCID			NUMERIC(18,0),  " +
                "   	@TenantID		NUMERIC(18,0),  " +
                "   	@BarCode_out	VARCHAR(50) OUTPUT  " +
                "   )  " +
                "   AS  " +
                "   BEGIN  " +
                "   	DECLARE @BatchID		NUMERIC(18,0)  " +
                "   	DECLARE @StockID		NUMERIC(18,0)  " +
                "   	DECLARE @LastInvDt		DATETIME = Getdate()  " +
                "   	DECLARE @STOCKHISID		NUMERIC(18,0)  " +
                "   	DECLARE @PRFXBATCH		VARCHAR(10)  " +
                "   	DECLARE @Stock		   NUMERIC(18,5)  " +
                "   	DECLARE @OldQTY         NUMERIC(18,5)  " +
                "   	DECLARE @INVID          NUMERIC(18,0)  " +
                "   	DECLARE @BarCode		VARCHAR(50)  " +
                "   	DECLARE @BarUniq		VARCHAR(100)  " +
                "   	DECLARE @CalcQOH		NUMERIC(18,5)  " +
                "   	DECLARE @BLNADVANCED	INT  " +
                "   	DECLARE @blnExpiry		BIT  " +
                "   	SET @BarCode = @BatchCode  " +
                "   	SELECT @StockID = ISNULL(MAX(StockID) + 1,0) FROM tblStock WHERE TenantID = @TenantID  " +
                "   	SELECT @BatchID = ISNULL(MAX(BatchID) + 1,0) FROM tblStock WHERE TenantID = @TenantID  " +
                "   	SELECT @STOCKHISID = ISNULL(MAX(STOCKHISID) + 1,0) FROM tblStockHistory WHERE TenantID = @TenantID  " +
                "   	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED'   " +
                "   	SELECT @Stock = ISNULL(QOH,0)FROM tblStock WHERE ItemID= @ItemID AND BatchCode = @BatchCode AND TenantID = @TenantID  " +
                "   	SELECT @INVID = invID FROM tblPurchase WHERE ReferenceAutoNO=@RefID  " +
                "   	SELECT @OldQTY = ISNULL(Qty,0)FROM tblPurchaseItem WHERE ItemID= @ItemID AND InvID= @INVID  " +
                "   	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   	IF @StockID = 0  " +
                "   	BEGIN  " +
                "   		SET @StockID = 1  " +
                "   	END  " +
                "   	IF @STOCKHISID = 0  " +
                "   	BEGIN  " +
                "   		SET @STOCKHISID = 1  " +
                "   	END  " +
                "   	IF @BatchID = 0  " +
                "   	BEGIN  " +
                "   		SET @BatchID = 1  " +
                "   	END  " +
                "   	IF @Action = 'STOCKADD'  " +
                "   	BEGIN  " +
                "   		IF @BatchCode = '<Auto Barcode>'  " +
                "   		BEGIN  " +
                "   			Declare @Prefix VARCHAR(50)  " +
                "   			Declare @BatchPrefix VARCHAR(50)  " +
                "   			IF @BLNADVANCED = 1  " +
                "   			BEGIN  " +
                "   				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "   				set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "   				IF(@BatchPrefix='<YEARMONTH>')  " +
                "   				BEGIN  " +
                "   					SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))  " +
                "   					SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "   					set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "   					SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   					SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   			END  " +
                "   			IF @blnExpiry = 1  " +
                "   			BEGIN  " +
                "   				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))   " +
                "   				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))   " +
                "   			END  " +
                "   			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "   			BEGIN  " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID  AND CCID = @CCID)  " +
                "   				BEGIN  " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   				ELSE   " +
                "   				BEGIN  " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   		END  " +
                "   		ELSE  " +
                "   		BEGIN  " +
                "   			IF @BatchMode = 0   " +
                "   			BEGIN  " +
                "   				SELECT @BatchCode = ItemCode FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   				SET @BatchUniq = @BatchCode  " +
                "   				SET @BarCode = @BatchCode  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				if CHARINDEX('@',@BatchUniq) = 0  " +
                "   				BEGIN  " +
                "   					IF @blnExpiry = 1  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   					END  " +
                "   					ELSE  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "   			BEGIN  " +
                "   				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   		END  " +
                "   		SET @BatchCode = @BarCode  " +
                "   	END  " +
                "   	IF @Action = 'STOCKLESS'  " +
                "   	BEGIN  " +
                "   		SET @Qty = @Qty * -1;  " +
                "   	END  " +
                "   	IF @Action = 'STOCKDEL'  " +
                "   	BEGIN  " +
                "   		SELECT @CalcQOH = QOH FROM tblStock WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "   		UPDATE tblStock SET QOH = QOH - @CalcQOH WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "   		DELETE FROM tblStockHistory WHERE RefId = @RefID AND CCID = @CCID AND TenantID = @TenantID  " +
                "   	END  " +
                "   	SET @BarCode_out = @BatchUniq  " +
                "   	SELECT @BarCode_out  " +
                "   END  ";
                Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdateFromItem') " +
            "DROP PROCEDURE UspTransStockUpdateFromItem";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspTransStockUpdateFromItem]  " +
                "   (  " +
                "   	@ItemID			NUMERIC(18,0),  " +
                "   	@BatchCode		VARCHAR(50),  " +
                "   	@BatchUniq		VARCHAR(50),  " +
                "   	@Qty			NUMERIC(18,5),  " +
                "   	@MRP			NUMERIC(18,5),  " +
                "   	@CostRateInc	NUMERIC(18,5),  " +
                "   	@CostRateExcl	NUMERIC(18,5),  " +
                "   	@PRateExcl		NUMERIC(18,5),  " +
                "   	@PrateInc		NUMERIC(18,5),  " +
                "   	@TaxPer			NUMERIC(18,5),  " +
                "   	@SRate1			NUMERIC(18,5),  " +
                "   	@SRate2			NUMERIC(18,5),  " +
                "   	@SRate3			NUMERIC(18,5),  " +
                "   	@SRate4			NUMERIC(18,5),  " +
                "   	@SRate5			NUMERIC(18,5),  " +
                "   	@BatchMode		INT,  " +
                "   	@VchType		VARCHAR(100),  " +
                "   	@VchDate		DATETIME,  " +
                "   	@ExpDt			DATETIME,  " +
                "   	@Action	 	    VARCHAR(20),  " +
                "   	@RefID			NUMERIC(18,0),  " +
                "   	@VchTypeID		NUMERIC(18,0),  " +
                "   	@CCID			NUMERIC(18,0),  " +
                "   	@TenantID		NUMERIC(18,0)  " +
                "   )  " +
                "   AS  " +
                "   BEGIN  " +
                "   	DECLARE @BatchID		NUMERIC(18,0)  " +
                "   	DECLARE @StockID		NUMERIC(18,0)  " +
                "   	DECLARE @LastInvDt		DATETIME = Getdate()  " +
                "   	DECLARE @STOCKHISID		NUMERIC(18,0)  " +
                "   	DECLARE @PRFXBATCH		VARCHAR(10)  " +
                "   	DECLARE @Stock		   NUMERIC(18,5)  " +
                "   	DECLARE @OldQTY         NUMERIC(18,5)  " +
                "   	DECLARE @INVID          NUMERIC(18,0)  " +
                "   	DECLARE @BarCode		VARCHAR(50)  " +
                "   	DECLARE @BarUniq		VARCHAR(100)  " +
                "   	DECLARE @CalcQOH		NUMERIC(18,5)  " +
                "   	DECLARE @BLNADVANCED	INT  " +
                "   	DECLARE @blnExpiry		BIT  " +
                "   	SET @BarCode = @BatchCode  " +
                "   	SELECT @StockID = ISNULL(MAX(StockID) + 1,0) FROM tblStock WHERE TenantID = @TenantID  " +
                "   	SELECT @BatchID = ISNULL(MAX(BatchID) + 1,0) FROM tblStock WHERE TenantID = @TenantID  " +
                "   	SELECT @STOCKHISID = ISNULL(MAX(STOCKHISID) + 1,0) FROM tblStockHistory WHERE TenantID = @TenantID  " +
                "   	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED'   " +
                "   	SELECT @Stock = ISNULL(QOH,0)FROM tblStock WHERE ItemID= @ItemID AND BatchCode = @BatchCode AND TenantID = @TenantID  " +
                "   	SELECT @INVID = invID FROM tblPurchase WHERE ReferenceAutoNO=@RefID  " +
                "   	SELECT @OldQTY = ISNULL(Qty,0)FROM tblPurchaseItem WHERE ItemID= @ItemID AND InvID= @INVID  " +
                "   	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   	IF @StockID = 0  " +
                "   	BEGIN  " +
                "   		SET @StockID = 1  " +
                "   	END  " +
                "   	IF @STOCKHISID = 0  " +
                "   	BEGIN  " +
                "   		SET @STOCKHISID = 1  " +
                "   	END  " +
                "   	IF @BatchID = 0  " +
                "   	BEGIN  " +
                "   		SET @BatchID = 1  " +
                "   	END  " +
                "   	IF @Action = 'STOCKADD'  " +
                "   	BEGIN  " +
                "   		IF @BatchCode = '<Auto Barcode>'  " +
                "   		BEGIN  " +
                "   			Declare @Prefix VARCHAR(50)  " +
                "   			Declare @BatchPrefix VARCHAR(50)  " +
                "   			IF @BLNADVANCED = 1  " +
                "   			BEGIN  " +
                "   				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "   				set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "   				IF(@BatchPrefix='<YEARMONTH>')  " +
                "   				BEGIN  " +
                "   					SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))  " +
                "   					SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "   					set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "   					SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   					SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   			END  " +
                "   			SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   			SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "   			BEGIN  " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID  AND CCID = @CCID)   " +
                "   				BEGIN  " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode  " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   		END  " +
                "   		ELSE  " +
                "   		BEGIN  " +
                "   			IF @BatchMode = 0   " +
                "   			BEGIN  " +
                "   				SELECT @BatchCode = ItemCode FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   				SET @BatchUniq = @BatchCode  " +
                "   				SET @BarCode = @BatchCode  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   			SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "   				IF CHARINDEX('@',@BatchUniq) = 0  " +
                "   				BEGIN  " +
                "   					IF @blnExpiry = 1  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   					END  " +
                "   					ELSE  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "   			BEGIN  " +
                "   				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   		END  " +
                "   		SET @BatchCode = @BarCode  " +
                "   	END  " +
                "   	IF @Action = 'STOCKLESS'  " +
                "   	BEGIN  " +
                "   		SET @Qty = @Qty * -1;  " +
                "   	END  " +
                "   	IF @Action = 'STOCKDEL'  " +
                "   	BEGIN  " +
                "   		SELECT @CalcQOH = QOH FROM tblStock WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "   		UPDATE tblStock SET QOH = QOH - @CalcQOH WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "   		DELETE FROM tblStockHistory WHERE RefId = @RefID AND ItemID = @ItemID AND BatchCode = @BatchCode AND VchTypeID = @VchTypeID AND CCID = @CCID AND TenantID = @TenantID  " +
                "   	END  " +
                "   	SELECT @BatchCode  " +
                "   END  ";
                Comm.fnExecuteNonQuery(sQuery, false);

            //11-Apr-2022 5.24 PM ------------------------------------ >>

            sQuery = "   CREATE NONCLUSTERED INDEX UK_BatchUnique_CCID_TenantID ON dbo.tblStock  " +
                "   	(  " +
                "   	BatchUnique,  " +
                "   	CCID,  " +
                "   	TenantID  " +
                "   	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]  ";
             Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   ALTER TABLE dbo.tblStock SET (LOCK_ESCALATION = TABLE)  ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdate') " +
            "DROP PROCEDURE UspTransStockUpdate";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspTransStockUpdate]  " +
                "   (  " +
                "   	@ItemID			NUMERIC(18,0),  " +
                "   	@BatchCode		VARCHAR(50),  " +
                "   	@BatchUniq		VARCHAR(50),  " +
                "   	@Qty			NUMERIC(18,5),  " +
                "   	@MRP			NUMERIC(18,5),  " +
                "   	@CostRateInc	NUMERIC(18,5),  " +
                "   	@CostRateExcl	NUMERIC(18,5),  " +
                "   	@PRateExcl		NUMERIC(18,5),  " +
                "   	@PrateInc		NUMERIC(18,5),  " +
                "   	@TaxPer			NUMERIC(18,5),  " +
                "   	@SRate1			NUMERIC(18,5),  " +
                "   	@SRate2			NUMERIC(18,5),  " +
                "   	@SRate3			NUMERIC(18,5),  " +
                "   	@SRate4			NUMERIC(18,5),  " +
                "   	@SRate5			NUMERIC(18,5),  " +
                "   	@BatchMode		INT,  " +
                "   	@VchType		VARCHAR(100),  " +
                "   	@VchDate		DATETIME,  " +
                "   	@ExpDt			DATETIME,  " +
                "   	@Action	 	    VARCHAR(20),  " +
                "   	@RefID			NUMERIC(18,0),  " +
                "   	@VchTypeID		NUMERIC(18,0),  " +
                "   	@CCID			NUMERIC(18,0),  " +
                "   	@TenantID		NUMERIC(18,0),  " +
                "   	@BarCode_out	VARCHAR(50) OUTPUT  " +
                "   )  " +
                "   AS  " +
                "   BEGIN  " +
                "   	DECLARE @BatchID		NUMERIC(18,0)  " +
                "   	DECLARE @StockID		NUMERIC(18,0)  " +
                "   	DECLARE @LastInvDt		DATETIME = Getdate()  " +
                "   	DECLARE @STOCKHISID		NUMERIC(18,0)  " +
                "   	DECLARE @PRFXBATCH		VARCHAR(10)  " +
                "   	DECLARE @Stock		   NUMERIC(18,5)  " +
                "   	DECLARE @INVID          NUMERIC(18,0)  " +
                "   	DECLARE @BarCode		VARCHAR(50)  " +
                "   	DECLARE @BarUniq		VARCHAR(100)  " +
                "   	DECLARE @CalcQOH		NUMERIC(18,5)  " +
                "   	DECLARE @BLNADVANCED	INT  " +
                "   	DECLARE @blnExpiry		BIT  " +
                "   	DECLARE @LessQty		NUMERIC(18,5)  " +
                "   	SET @BarCode = @BatchCode  " +
                "   	SELECT @StockID = MAX(ISNULL(StockID,0)) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   	SELECT @BatchID = MAX(ISNULL(BatchID,0)) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   	SELECT @STOCKHISID = MAX(ISNULL(STOCKHISID,0)) + 1 FROM tblStockHistory WHERE TenantID = @TenantID  " +
                "   	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED'   " +
                "   	SELECT @Stock = ISNULL(QOH,0)FROM tblStock WHERE ItemID= @ItemID AND BatchCode = @BatchCode AND TenantID = @TenantID  " +
                "   	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   	SET @LessQty = -1  " +
                "   	IF @StockID = 0  " +
                "   	BEGIN  " +
                "   		SET @StockID = 1  " +
                "   	END  " +
                "   	IF @STOCKHISID = 0  " +
                "   	BEGIN  " +
                "   		SET @STOCKHISID = 1  " +
                "   	END  " +
                "   	IF @BatchID = 0  " +
                "   	BEGIN  " +
                "   		SET @BatchID = 1  " +
                "   	END  " +
                "   	IF @Action = 'STOCKADD'  " +
                "   	BEGIN  " +
                "   		IF @BatchCode = '<Auto Barcode>'  " +
                "   		BEGIN  " +
                "   			Declare @Prefix VARCHAR(50)  " +
                "   			Declare @BatchPrefix VARCHAR(50)  " +
                "   			IF @BLNADVANCED = 1  " +
                "   			BEGIN  " +
                "   				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "   				set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "   				IF(@BatchPrefix='<YEARMONTH>')  " +
                "   				BEGIN  " +
                "   					SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))  " +
                "   					SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "   					set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "   					SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   					IF @BatchPrefix <> ''  " +
                "   					BEGIN  " +
                "   						SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "   			END  " +
                "   			SET @BarCode =  @BatchCode  " +
                "   			IF @blnExpiry = 1  " +
                "   			BEGIN  " +
                "   				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))   " +
                "   				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))   " +
                "   			END  " +
                "   			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)  " +
                "   			BEGIN  " +
                "   				IF @VchTypeID <> 0  " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID  AND CCID = @CCID)  " +
                "   				BEGIN  " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   		END  " +
                "   		ELSE  " +
                "   		BEGIN  " +
                "   			IF @BatchMode = 0   " +
                "   			BEGIN  " +
                "   				SELECT @BatchCode = ItemCode FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   				SET @BarCode = @BatchCode  " +
                "   				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   			ELSE  " +
                "   			BEGIN  " +
                "   				IF CHARINDEX('@',@BatchUniq) = 0  " +
                "   				BEGIN  " +
                "   					IF @blnExpiry = 1  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   					END  " +
                "   					ELSE  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "   					END  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					IF @blnExpiry = 1  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "   					END  " +
                "   					ELSE  " +
                "   					BEGIN  " +
                "   						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "   					END  " +
                "   				END  " +
                "   				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID AND MRP=@MRP AND FORMAT(ExpiryDate,'dd-MM-yy') = FORMAT(@ExpDt,'dd-MM-yy'))   " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   				ELSE  " +
                "   				BEGIN  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "   					IF @VchTypeID <> 0  " +
                "   					BEGIN  " +
                "   						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   					END  " +
                "   				END  " +
                "   			END  " +
                "   		END  " +
                "   		SET @BatchCode = @BarCode  " +
                "   	END  " +
                "   	IF @Action = 'STOCKLESS'  " +
                "   	BEGIN  " +
                "   		IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "   		BEGIN  " +
                "   			IF @VchTypeID <> 0  " +
                "   			BEGIN  " +
                "   				SET @LessQty = @LessQty * @Qty  " +
                "   				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@LessQty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "   				INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "   				VALUES(@VchType,@VchDate,@RefID,@ItemID,0,@Qty,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "   			END  " +
                "   		END  " +
                "   	END  " +
                "   	IF @Action = 'STOCKDEL'  " +
                "   	BEGIN  " +
                "   		SELECT @CalcQOH = QOH FROM tblStock WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "   		UPDATE tblStock SET QOH = QOH - @CalcQOH WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "   		DELETE FROM tblStockHistory WHERE RefId = @RefID AND CCID = @CCID AND TenantID = @TenantID  " +
                "   	END  " +
                "   	SET @BarCode_out = @BatchUniq  " +
                "   	SELECT @BarCode_out  " +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspStockInsert') " +
           "DROP PROCEDURE UspStockInsert";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspStockInsert]  " +
                "   (  " +
                "        @StockID    NUMERIC  (18,0),  " +
                "        @TenantID    NUMERIC  (18,0),  " +
                "        @CCID    NUMERIC  (18,0),  " +
                "        @BatchCode    VARCHAR(100),  " +
                "        @BatchUnique    VARCHAR  (50),  " +
                "        @BatchID    NUMERIC  (18,0),  " +
                "        @MRP    NUMERIC(18,5),  " +
                "        @ExpiryDate    DATE,  " +
                "        @CostRateInc    DECIMAL(18,2),  " +
                "        @CostRateExcl    DECIMAL(18,2),  " +
                "        @PRateExcl    DECIMAL(18,2),  " +
                "        @PrateInc    DECIMAL(18,2),  " +
                "        @TaxPer    DECIMAL(18,2),  " +
                "        @SRate1    DECIMAL(18,2),  " +
                "        @SRate2    DECIMAL(18,2),  " +
                "        @SRate3    DECIMAL(18,2),  " +
                "        @SRate4    DECIMAL(18,2),  " +
                "        @SRate5    DECIMAL(18,2),  " +
                "        @QOH    DECIMAL,  " +
                "        @LastInvDate    DATE,  " +
                "        @LastInvNo    VARCHAR  (50),  " +
                "        @LastSupplierID   NUMERIC  (18,0),  " +
                "   	 @Action             INT=0,  " +
                "   	 @ItemID		NUMERIC (18,0),  " +
                "   	 @BatchMode	VARCHAR(100)  " +
                "   )  " +
                "   AS  " +
                "   BEGIN  " +
                "   DECLARE @RetResult      INT  " +
                "   DECLARE @TransType		CHAR(1)  " +
                "   DECLARE @blnExpiry		NUMERIC(18,0)  " +
                "   BEGIN TRY  " +
                "   BEGIN TRANSACTION;  " +
                "   IF @BatchMode = 0   " +
                "   BEGIN  " +
                "   	SELECT @BatchCode = ItemCode,@blnExpiry= ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "   END  " +
                "   IF @Action = 0  " +
                "   BEGIN  " +
                "   		IF @BatchMode = 0  " +
                "   		BEGIN  " +
                "   			/*None*/  " +
                "   			INSERT INTO tblStock(StockID,TenantID,CCID,BatchCode,BatchUnique,BatchID,MRP,ExpiryDate,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,QOH,LastInvDate,LastInvNo,LastSupplierID,ItemID)  " +
                "   			VALUES(@StockID,@TenantID,@CCID,@BatchCode,@BatchUnique,@BatchID,@MRP,@ExpiryDate,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,ABS(@QOH),@LastInvDate,@LastInvNo,@LastSupplierID,@ItemID)  " +
                "   			SET @RetResult = 1;  " +
                "   			SET @TransType = 'S';  " +
                "   		END  " +
                "   		ELSE IF @BatchMode = 1  " +
                "   		BEGIN  " +
                "   			INSERT INTO tblStock(StockID,TenantID,CCID,BatchCode,BatchUnique,BatchID,MRP,ExpiryDate,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,QOH,LastInvDate,LastInvNo,LastSupplierID,ItemID)  " +
                "   			VALUES(@StockID,@TenantID,@CCID,@BatchCode,@BatchUnique,@BatchID,@MRP,@ExpiryDate,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,ABS(@QOH),@LastInvDate,@LastInvNo,@LastSupplierID,@ItemID)  " +
                "   			SET @RetResult = 1;  " +
                "   			SET @TransType = 'S';  " +
                "   		END  " +
                "   		ELSE IF @BatchMode = 2 AND @BatchCode <> ''   " +
                "   		BEGIN  " +
                "   			/*Auto*/  " +
                "   			INSERT INTO tblStock(StockID,TenantID,CCID,BatchCode,BatchUnique,BatchID,MRP,ExpiryDate,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,QOH,LastInvDate,LastInvNo,LastSupplierID,ItemID)  " +
                "   			VALUES(@StockID,@TenantID,@CCID,@BatchCode,@BatchUnique,@BatchID,@MRP,@ExpiryDate,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,ABS(@QOH),@LastInvDate,@LastInvNo,@LastSupplierID,@ItemID)  " +
                "   			SET @RetResult = 1;  " +
                "   			SET @TransType = 'S';  " +
                "   		END  " +
                "   		ELSE IF @BatchMode = 3  " +
                "   		BEGIN  " +
                "   			INSERT INTO tblStock(StockID,TenantID,CCID,BatchCode,BatchUnique,BatchID,MRP,ExpiryDate,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,QOH,LastInvDate,LastInvNo,LastSupplierID,ItemID)  " +
                "   			VALUES(@StockID,@TenantID,@CCID,@BatchCode,@BatchUnique,@BatchID,@MRP,@ExpiryDate,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,ABS(@QOH),@LastInvDate,@LastInvNo,@LastSupplierID,@ItemID)  " +
                "   			SET @RetResult = 1;  " +
                "   			SET @TransType = 'S';  " +
                "   		END  " +
                "   END  " +
                "   IF @Action = 1  " +
                "   BEGIN  " +
                "   		UPDATE tblStock SET BatchID=@BatchID,MRP=@MRP,ExpiryDate=@ExpiryDate,CostRateInc=@CostRateInc,CostRateExcl=@CostRateExcl,PRateExcl=@PRateExcl,PrateInc=@PrateInc,TaxPer=@TaxPer,SRate1=@SRate1,SRate2=@SRate2,SRate3=@SRate3,SRate4=@SRate4,SRate5=@SRate5,QOH=QOH + @QOH,LastInvDate=@LastInvDate,LastInvNo=@LastInvNo,LastSupplierID=@LastSupplierID  " +
                "   		WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUnique AND TenantID=@TenantID  " +
                "   		SET @RetResult = 1;  " +
                "   		SET @TransType = 'E';  " +
                "   END  " +
                "   IF @Action = 2  " +
                "   BEGIN  " +
                "   		UPDATE tblStock SET CCID=@CCID,BatchCode=@BatchCode,BatchUnique=@BatchUnique,BatchID=@BatchID,MRP=@MRP,ExpiryDate=@ExpiryDate,CostRateInc=@CostRateInc,CostRateExcl=@CostRateExcl,PRateExcl=@PRateExcl,PrateInc=@PrateInc,TaxPer=@TaxPer,SRate1=@SRate1,SRate2=@SRate2,SRate3=@SRate3,SRate4=@SRate4,SRate5=@SRate5,QOH=QOH + @QOH,LastInvDate=@LastInvDate,LastInvNo=@LastInvNo,LastSupplierID=@LastSupplierID  " +
                "   		WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUnique AND TenantID=@TenantID  " +
                "   		SET @RetResult = 0;  " +
                "   		SET @TransType = 'D';  " +
                "   END  " +
                "   COMMIT TRANSACTION;  " +
                "   SELECT @RetResult as SqlSpResult,@StockID as TransID,@TransType as TransactType  " +
                "   END TRY  " +
                "   BEGIN CATCH  " +
                "   ROLLBACK;  " +
                "   SELECT  " +
                "   - 1 as SqlSpResult,  " +
                "   ERROR_NUMBER() AS ErrorNumber,  " +
                "   ERROR_STATE() AS ErrorState,  " +
                "   ERROR_SEVERITY() AS ErrorSeverity,  " +
                "   ERROR_PROCEDURE() AS ErrorProcedure,  " +
                "   ERROR_LINE() AS ErrorLine,  " +
                "   ERROR_MESSAGE() AS ErrorMessage;  " +
                "   END CATCH;  " +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseItemInsert') " +
           "DROP PROCEDURE UspPurchaseItemInsert";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspPurchaseItemInsert]( @InvID NUMERIC (18, 0), @ItemId NUMERIC (18, 0), @Qty FLOAT, @Rate FLOAT, @UnitId NUMERIC (18, 0), @Batch VARCHAR (50), @TaxPer FLOAT, @TaxAmount FLOAT, @Discount FLOAT, @MRP FLOAT, @SlNo NUMERIC (18, 0), @Prate FLOAT, @Free FLOAT, @SerialNos VARCHAR (5000), @ItemDiscount FLOAT, @BatchCode VARCHAR (50), @iCessOnTax FLOAT, @blnCessOnTax NUMERIC (18, 0), @Expiry DATETIME, @ItemDiscountPer FLOAT, @RateInclusive NUMERIC (18, 0), @ITaxableAmount FLOAT, @INetAmount FLOAT, @CGSTTaxPer FLOAT, @CGSTTaxAmt FLOAT, @SGSTTaxPer FLOAT, @SGSTTaxAmt FLOAT, @IGSTTaxPer FLOAT, @IGSTTaxAmt FLOAT, @iRateDiscPer FLOAT, @iRateDiscount FLOAT, @BatchUnique VARCHAR (150), @blnQtyIN NUMERIC (18, 0), @CRate FLOAT, @Unit VARCHAR (50), @ItemStockID NUMERIC (18, 0), @IcessPercent FLOAT, @IcessAmt FLOAT, @IQtyCompCessPer FLOAT, @IQtyCompCessAmt FLOAT, @StockMRP FLOAT, @BaseCRate FLOAT, @InonTaxableAmount FLOAT, @IAgentCommPercent FLOAT, @BlnDelete NUMERIC (18, 0), @Id NUMERIC (18, 0), @StrOfferDetails VARCHAR (100), @BlnOfferItem FLOAT, @BalQty FLOAT, @GrossAmount FLOAT, @iFloodCessPer FLOAT, @iFloodCessAmt FLOAT, @Srate1 FLOAT, @Srate2 FLOAT, @Srate3 FLOAT, @Srate4 FLOAT, @Srate5 FLOAT, @Costrate FLOAT, @CostValue FLOAT, @Profit FLOAT, @ProfitPer FLOAT, @DiscMode NUMERIC (18, 0), @Srate1Per FLOAT, @Srate2Per FLOAT, @Srate3Per FLOAT, @Srate4Per FLOAT, @Srate5Per FLOAT, @Action INT = 0) AS   " +
                "   BEGIN  " +
                "      DECLARE @RetResult INT   " +
                "      DECLARE @RetID INT   " +
                "      DECLARE @VchType VARCHAR(50)   " +
                "      DECLARE @VchTypeID NUMERIC(18, 0)   " +
                "      DECLARE @BatchMode VARCHAR(50)   " +
                "      DECLARE @VchDate DATETIME   " +
                "      DECLARE @CCID NUMERIC(18, 0)   " +
                "      DECLARE @TenantID NUMERIC(18, 0)   " +
                "      DECLARE @BarCode_out VARCHAR(50)   " +
                "      DECLARE @VchParentID	NUMERIC(18, 0)   " +
                "      BEGIN  " +
                "         TRY   " +
                "         BEGIN  " +
                "            TRANSACTION;  " +
                "   SELECT  " +
                "      @VchType = VchType,  " +
                "      @VchTypeID = VchTypeID,  " +
                "      @VchDate = InvDate,  " +
                "      @CCID = CCID,  " +
                "      @TenantID = TenantID   " +
                "   FROM  " +
                "      tblPurchase   " +
                "   WHERE  " +
                "      InvId = @InvID   " +
                "      SELECT  " +
                "         @BatchMode = BatchMode   " +
                "      FROM  " +
                "         tblItemMaster   " +
                "      WHERE  " +
                "         ItemID = @ItemId   " +
                "   	  SELECT @VchParentID = ParentID FROM tblVchType WHERE VchTypeID = @VchTypeID  " +
                "   	  IF @Action = 0   " +
                "         BEGIN  " +
                "   		IF @VchParentID = 2  " +
                "   		BEGIN  " +
                "            EXEC UspTransStockUpdate @ItemId,  " +
                "            @BatchCode,  " +
                "            @BatchUnique,  " +
                "            @Qty,  " +
                "            @MRP,  " +
                "            @CRate,  " +
                "            @CRate,  " +
                "            @Prate,  " +
                "            @Prate,  " +
                "            @TaxPer,  " +
                "            @Srate1,  " +
                "            @Srate2,  " +
                "            @Srate3,  " +
                "            @Srate4,  " +
                "            @Srate5,  " +
                "            @BatchMode,  " +
                "            @VchType,  " +
                "            @VchDate,  " +
                "            @Expiry,  " +
                "            'STOCKADD',  " +
                "            @InvID,  " +
                "            @VchTypeID,  " +
                "            @CCID,  " +
                "            @TenantID,  " +
                "            @BarCode_out output   " +
                "   		 IF CHARINDEX('@', @BarCode_out) > 0   " +
                "            BEGIN  " +
                "   		   SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))   " +
                "            END  " +
                "   		END  " +
                "   		ELSE IF @VchParentID = 4  " +
                "   		BEGIN  " +
                "   			EXEC UspTransStockUpdate @ItemId,@BatchCode,@BatchUnique,@Qty,@MRP,@CRate,@CRate,@Prate,@Prate,@TaxPer,@Srate1,@Srate2,@Srate3,@Srate4,@Srate5,@BatchMode,@VchType,@VchDate,@Expiry,'STOCKLESS',@InvID,@VchTypeID,@CCID,@TenantID,@BarCode_out output   " +
                "   			IF CHARINDEX('@', @BarCode_out) > 0   " +
                "   			BEGIN  " +
                "   			SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))   " +
                "   			END  " +
                "   		END  " +
                "           ELSE  " +
                "           BEGIN  " +
                "   		SET @BatchCode = @BarCode_out   " +
                "           END  " +
                "               INSERT INTO  " +
                "                  tblPurchaseItem(InvID, ItemId, Qty, Rate, UnitId, Batch, TaxPer, TaxAmount, Discount, MRP, SlNo, Prate, Free, SerialNos, ItemDiscount, BatchCode, iCessOnTax, blnCessOnTax, Expiry, ItemDiscountPer, RateInclusive, ITaxableAmount, INetAmount, CGSTTaxPer, CGSTTaxAmt, SGSTTaxPer, SGSTTaxAmt, IGSTTaxPer, IGSTTaxAmt, iRateDiscPer, iRateDiscount, BatchUnique, blnQtyIN, CRate, Unit, ItemStockID, IcessPercent, IcessAmt, IQtyCompCessPer, IQtyCompCessAmt, StockMRP, BaseCRate, InonTaxableAmount, IAgentCommPercent, BlnDelete, StrOfferDetails, BlnOfferItem, BalQty, GrossAmount, iFloodCessPer, iFloodCessAmt, Srate1, Srate2, Srate3, Srate4, Srate5, Costrate, CostValue, Profit, ProfitPer, DiscMode, Srate1Per, Srate2Per, Srate3Per, Srate4Per, Srate5Per)   " +
                "               VALUES  " +
                "                  (  " +
                "                     @InvID,  " +
                "                     @ItemId,  " +
                "                     @Qty,  " +
                "                     @Rate,  " +
                "                     @UnitId,  " +
                "                     @Batch,  " +
                "                     @TaxPer,  " +
                "                     @TaxAmount,  " +
                "                     @Discount,  " +
                "                     @MRP,  " +
                "                     @SlNo,  " +
                "                     @Prate,  " +
                "                     @Free,  " +
                "                     @SerialNos,  " +
                "                     @ItemDiscount,  " +
                "                     @BatchCode,  " +
                "                     @iCessOnTax,  " +
                "                     @blnCessOnTax,  " +
                "                     @Expiry,  " +
                "                     @ItemDiscountPer,  " +
                "                     @RateInclusive,  " +
                "                     @ITaxableAmount,  " +
                "                     @INetAmount,  " +
                "                     @CGSTTaxPer,  " +
                "                     @CGSTTaxAmt,  " +
                "                     @SGSTTaxPer,  " +
                "                     @SGSTTaxAmt,  " +
                "                     @IGSTTaxPer,  " +
                "                     @IGSTTaxAmt,  " +
                "                     @iRateDiscPer,  " +
                "                     @iRateDiscount,  " +
                "                     @BarCode_out,  " +
                "                     @blnQtyIN,  " +
                "                     @CRate,  " +
                "                     @Unit,  " +
                "                     @ItemStockID,  " +
                "                     @IcessPercent,  " +
                "                     @IcessAmt,  " +
                "                     @IQtyCompCessPer,  " +
                "                     @IQtyCompCessAmt,  " +
                "                     @StockMRP,  " +
                "                     @BaseCRate,  " +
                "                     @InonTaxableAmount,  " +
                "                     @IAgentCommPercent,  " +
                "                     @BlnDelete,  " +
                "                     @StrOfferDetails,  " +
                "                     @BlnOfferItem,  " +
                "                     @BalQty,  " +
                "                     @GrossAmount,  " +
                "                     @iFloodCessPer,  " +
                "                     @iFloodCessAmt,  " +
                "                     @Srate1,  " +
                "                     @Srate2,  " +
                "                     @Srate3,  " +
                "                     @Srate4,  " +
                "                     @Srate5,  " +
                "                     @Costrate,  " +
                "                     @CostValue,  " +
                "                     @Profit,  " +
                "                     @ProfitPer,  " +
                "                     @DiscMode,  " +
                "                     @Srate1Per,  " +
                "                     @Srate2Per,  " +
                "                     @Srate3Per,  " +
                "                     @Srate4Per,  " +
                "                     @Srate5Per  " +
                "                  )  " +
                "               SET  " +
                "                  @RetResult = 1;  " +
                "         END  " +
                "         ELSE  " +
                "            IF @Action = 2   " +
                "            BEGIN  " +
                "               EXEC UspTransStockUpdate @ItemId,  " +
                "               @BatchCode,  " +
                "               @BatchUnique,  " +
                "               @Qty,  " +
                "               @MRP,  " +
                "               @CRate,  " +
                "               @CRate,  " +
                "               @Prate,  " +
                "               @Prate,  " +
                "               @TaxPer,  " +
                "               @Srate1,  " +
                "               @Srate2,  " +
                "               @Srate3,  " +
                "               @Srate4,  " +
                "               @Srate5,  " +
                "               @BatchMode,  " +
                "               @VchType,  " +
                "               @VchDate,  " +
                "               @Expiry,  " +
                "               'STOCKDEL',  " +
                "               @InvID,  " +
                "               @VchTypeID,  " +
                "               @CCID,  " +
                "               @TenantID,  " +
                "               @BarCode_out output   " +
                "               DELETE  " +
                "               FROM  " +
                "                  tblPurchaseItem   " +
                "               WHERE  " +
                "                  InvID = @InvID   " +
                "               SET  " +
                "                  @RetResult = 0;  " +
                "            END  " +
                "            ELSE  " +
                "               IF @Action = 3   " +
                "               BEGIN  " +
                "                  EXEC UspTransStockUpdate @ItemId,  " +
                "                  @BatchCode,  " +
                "                  @BatchUnique,  " +
                "                  @Qty,  " +
                "                  @MRP,  " +
                "                  @CRate,  " +
                "                  @CRate,  " +
                "                  @Prate,  " +
                "                  @Prate,  " +
                "                  @TaxPer,  " +
                "                  @Srate1,  " +
                "                  @Srate2,  " +
                "                  @Srate3,  " +
                "                  @Srate4,  " +
                "                  @Srate5,  " +
                "                  @BatchMode,  " +
                "                  @VchType,  " +
                "                  @VchDate,  " +
                "                  @Expiry,  " +
                "                  'STOCKDEL',  " +
                "                  @InvID,  " +
                "                  @VchTypeID,  " +
                "                  @CCID,  " +
                "                  @TenantID,  " +
                "                  @BarCode_out output   " +
                "               SET  " +
                "                  @RetResult = 0;  " +
                "               END  " +
                "               COMMIT TRANSACTION;  " +
                "   SELECT  " +
                "      @RetResult as SqlSpResult,  " +
                "      @RetID as PID   " +
                "         END  " +
                "         TRY   " +
                "         BEGIN  " +
                "            CATCH ROLLBACK;  " +
                "   SELECT  " +
                "      - 1 as SqlSpResult,  " +
                "      @RetID as PID,  " +
                "      ERROR_NUMBER() AS ErrorNumber,  " +
                "      ERROR_STATE() AS ErrorState,  " +
                "      ERROR_SEVERITY() AS ErrorSeverity,  " +
                "      ERROR_PROCEDURE() AS ErrorProcedure,  " +
                "      ERROR_LINE() AS ErrorLine,  " +
                "      ERROR_MESSAGE() AS ErrorMessage;  " +
                "         END  " +
                "         CATCH;  " +
                "      END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdateFromItem') " +
          "DROP PROCEDURE UspTransStockUpdateFromItem";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspTransStockUpdateFromItem]       " +
                "   (     	@ItemID			NUMERIC(18,0),     	@BatchCode		VARCHAR(50),     	@BatchUniq		VARCHAR(50),     	@Qty			NUMERIC(18,5),     	@MRP			NUMERIC(18,5),     	@CostRateInc	NUMERIC(18,5),     	@CostRateExcl	NUMERIC(18,5),     	@PRateExcl		NUMERIC(18,5),     	@PrateInc		NUMERIC(18,5),     	@TaxPer			NUMERIC(18,5),     	@SRate1			NUMERIC(18,5),     	@SRate2			NUMERIC(18,5),     	@SRate3			NUMERIC(18,5),     	@SRate4			NUMERIC(18,5),     	@SRate5			NUMERIC(18,5),     	@BatchMode		INT,     	@VchType		VARCHAR(100),     	@VchDate		DATETIME,     	@ExpDt			DATETIME,     	@Action	 	    VARCHAR(20),     	@RefID			NUMERIC(18,0),     	@VchTypeID		NUMERIC(18,0),     	@CCID			NUMERIC(18,0),     	@TenantID		NUMERIC(18,0)     )       " +
                "   AS       " +
                "   BEGIN     " +
                "   	DECLARE @BatchID		NUMERIC(18,0)     	  " +
                "   	DECLARE @StockID		NUMERIC(18,0)     	  " +
                "   	DECLARE @LastInvDt		DATETIME = Getdate()     	  " +
                "   	DECLARE @STOCKHISID		NUMERIC(18,0)     	  " +
                "   	DECLARE @PRFXBATCH		VARCHAR(10)     	  " +
                "   	DECLARE @Stock		   NUMERIC(18,5)     	     	  " +
                "   	DECLARE @BarCode		VARCHAR(50)     	  " +
                "   	DECLARE @BarUniq		VARCHAR(100)     	  " +
                "   	DECLARE @CalcQOH		NUMERIC(18,5)     	  " +
                "   	DECLARE @BLNADVANCED	INT     	  " +
                "   	DECLARE @blnExpiry		BIT     	  " +
                "   	SET @BarCode = @BatchCode     	SELECT @StockID = ISNULL(MAX(StockID) + 1,0) FROM tblStock WHERE TenantID = @TenantID     	  " +
                "   	SELECT @BatchID = ISNULL(MAX(BatchID) + 1,0) FROM tblStock WHERE TenantID = @TenantID     	  " +
                "   	SELECT @STOCKHISID = ISNULL(MAX(STOCKHISID) + 1,0) FROM tblStockHistory WHERE TenantID = @TenantID     	  " +
                "   	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED'      	  " +
                "   	SELECT @Stock = ISNULL(QOH,0)FROM tblStock WHERE ItemID= @ItemID AND BatchCode = @BatchCode AND TenantID = @TenantID     	  	  " +
                "   	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID     	  " +
                "   	IF @StockID = 0     	  " +
                "   	BEGIN     		  " +
                "   		SET @StockID = 1     	  " +
                "   	END     	  " +
                "   	IF @STOCKHISID = 0     	  " +
                "   	BEGIN     		  " +
                "   		SET @STOCKHISID = 1     	  " +
                "   	END     	  " +
                "   	IF @BatchID = 0     	  " +
                "   	BEGIN     		  " +
                "   		SET @BatchID = 1     	  " +
                "   	END     	  " +
                "   	IF @Action = 'STOCKADD'     	  " +
                "   	BEGIN     		  " +
                "   		IF @BatchCode = '<Auto Barcode>'     		  " +
                "   		BEGIN     			  " +
                "   		Declare @Prefix VARCHAR(50)     			  " +
                "   		Declare @BatchPrefix VARCHAR(50)     			  " +
                "   		IF @BLNADVANCED = 1     			  " +
                "   		BEGIN     				  " +
                "   			Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'     				  " +
                "   			set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))     				  " +
                "   			IF(@BatchPrefix='<YEARMONTH>')     				  " +
                "   			BEGIN     					  " +
                "   				SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))     					  " +
                "   				SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)     				  " +
                "   			END     				  " +
                "   			ELSE     				  " +
                "   			BEGIN     					  " +
                "   				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'     					  " +
                "   				set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))     					  " +
                "   				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID     					  " +
                "   				SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)     				  " +
                "   			END     		  " +
                "   		END     			  " +
                "   		ELSE     			  " +
                "   		BEGIN     				  " +
                "   			SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID     			  " +
                "   		END     			  " +
                "   		SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')     			  " +
                "   		SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')     			  " +
                "   		IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)      			  " +
                "   		BEGIN     				  " +
                "   			IF @VchTypeID <> 0     				  " +
                "   			BEGIN     					  " +
                "   				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode     					  " +
                "   				INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)     					  " +
                "   				VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)     				  " +
                "   			END     			  " +
                "   		END     			  " +
                "   		ELSE     			  " +
                "   		BEGIN     				  " +
                "   			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID  AND CCID = @CCID)      				  " +
                "   			BEGIN     					  " +
                "   				IF @VchTypeID <> 0     					  " +
                "   				BEGIN     						  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode     						  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)     						  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)     					  " +
                "   				END     				  " +
                "   			END     				  " +
                "   			ELSE     				  " +
                "   			BEGIN     					  " +
                "   				IF @VchTypeID <> 0     					  " +
                "   				BEGIN     						  " +
                "   					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode      						  " +
                "   					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)     						  " +
                "   					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)     					  " +
                "   				END     				  " +
                "   			END     			  " +
                "   		END     		  " +
                "   	END     		  " +
                "   	ELSE     		  " +
                "   	BEGIN     			  " +
                "   		IF @BatchMode = 0      			  " +
                "   		BEGIN     				  " +
                "   			SELECT @BatchCode = ItemCode FROM tblItemMaster WHERE ItemID = @ItemID     				  " +
                "   			SET @BatchUniq = @BatchCode     				  " +
                "   			SET @BarCode = @BatchCode     			  " +
                "   		END     			  " +
                "   		ELSE     			  " +
                "   		BEGIN     			  " +
                "   			SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))     				  " +
                "   			IF CHARINDEX('@',@BatchUniq) = 0     				  " +
                "   			BEGIN     					  " +
                "   				IF @blnExpiry = 1     					  " +
                "   				BEGIN     						  " +
                "   					SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')     					  " +
                "   				END     					  " +
                "   				ELSE     					  " +
                "   				BEGIN     						  " +
                "   					SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))     					  " +
                "   				END     				  " +
                "   			END     			  " +
                "   		END     			  " +
                "   		IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)      			  " +
                "   		BEGIN     				  " +
                "   			EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode      				  " +
                "   			IF @VchTypeID <> 0     				  " +
                "   			BEGIN     					  " +
                "   				INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)     					  " +
                "   				VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)     				  " +
                "   			END     			  " +
                "   		END     			  " +
                "   		ELSE     			  " +
                "   		BEGIN     				  " +
                "   			EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode      				  " +
                "   			IF @VchTypeID <> 0     				  " +
                "   			BEGIN     					  " +
                "   				INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)     					  " +
                "   				VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)     				  " +
                "   			END     			  " +
                "   		END     		  " +
                "   	END     		  " +
                "   	SET @BatchCode = @BarCode     	  " +
                "   END     	IF @Action = 'STOCKLESS'     	  " +
                "   BEGIN     		  " +
                "   SET @Qty = @Qty * -1;     	  " +
                "   END     	  " +
                "   IF @Action = 'STOCKDEL'     	  " +
                "   BEGIN     		  " +
                "   	SELECT @CalcQOH = QOH FROM tblStock WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID     		  " +
                "   	UPDATE tblStock SET QOH = QOH - @CalcQOH WHERE ItemID = @ItemID AND CCID=@CCID AND BatchCode=@BatchCode AND BatchUnique=@BatchUniq AND TenantID=@TenantID     		  " +
                "   	DELETE FROM tblStockHistory WHERE RefId = @RefID AND ItemID = @ItemID AND BatchCode = @BatchCode AND VchTypeID = @VchTypeID AND CCID = @CCID AND TenantID = @TenantID     	  " +
                "   END     	  " +
                "   SELECT @BatchCode       " +
                "   END   ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseItemInsert') " +
          "DROP PROCEDURE UspPurchaseItemInsert";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspPurchaseItemInsert]( @InvID NUMERIC (18, 0), @ItemId NUMERIC (18, 0), @Qty FLOAT, @Rate FLOAT, @UnitId NUMERIC (18, 0), @Batch VARCHAR (50), @TaxPer FLOAT, @TaxAmount FLOAT, @Discount FLOAT, @MRP FLOAT, @SlNo NUMERIC (18, 0), @Prate FLOAT, @Free FLOAT, @SerialNos VARCHAR (5000), @ItemDiscount FLOAT, @BatchCode VARCHAR (50), @iCessOnTax FLOAT, @blnCessOnTax NUMERIC (18, 0), @Expiry DATETIME, @ItemDiscountPer FLOAT, @RateInclusive NUMERIC (18, 0), @ITaxableAmount FLOAT, @INetAmount FLOAT, @CGSTTaxPer FLOAT, @CGSTTaxAmt FLOAT, @SGSTTaxPer FLOAT, @SGSTTaxAmt FLOAT, @IGSTTaxPer FLOAT, @IGSTTaxAmt FLOAT, @iRateDiscPer FLOAT, @iRateDiscount FLOAT, @BatchUnique VARCHAR (150), @blnQtyIN NUMERIC (18, 0), @CRate FLOAT, @Unit VARCHAR (50), @ItemStockID NUMERIC (18, 0), @IcessPercent FLOAT, @IcessAmt FLOAT, @IQtyCompCessPer FLOAT, @IQtyCompCessAmt FLOAT, @StockMRP FLOAT, @BaseCRate FLOAT, @InonTaxableAmount FLOAT, @IAgentCommPercent FLOAT, @BlnDelete NUMERIC (18, 0), @Id NUMERIC (18, 0), @StrOfferDetails VARCHAR (100), @BlnOfferItem FLOAT, @BalQty FLOAT, @GrossAmount FLOAT, @iFloodCessPer FLOAT, @iFloodCessAmt FLOAT, @Srate1 FLOAT, @Srate2 FLOAT, @Srate3 FLOAT, @Srate4 FLOAT, @Srate5 FLOAT, @Costrate FLOAT, @CostValue FLOAT, @Profit FLOAT, @ProfitPer FLOAT, @DiscMode NUMERIC (18, 0), @Srate1Per FLOAT, @Srate2Per FLOAT, @Srate3Per FLOAT, @Srate4Per FLOAT, @Srate5Per FLOAT, @Action INT = 0) AS   " +
                "   BEGIN  " +
                "      DECLARE @RetResult INT   " +
                "      DECLARE @RetID INT   " +
                "      DECLARE @VchType VARCHAR(50)   " +
                "      DECLARE @VchTypeID NUMERIC(18, 0)   " +
                "      DECLARE @BatchMode VARCHAR(50)   " +
                "      DECLARE @VchDate DATETIME   " +
                "      DECLARE @CCID NUMERIC(18, 0)   " +
                "      DECLARE @TenantID NUMERIC(18, 0)   " +
                "      DECLARE @BarCode_out VARCHAR(50)   " +
                "      DECLARE @VchParentID NUMERIC(18, 0)   " +
                "      BEGIN  " +
                "         TRY   " +
                "         BEGIN  " +
                "            TRANSACTION;  " +
                "   SELECT  " +
                "      @VchType = VchType,  " +
                "      @VchTypeID = VchTypeID,  " +
                "      @VchDate = InvDate,  " +
                "      @CCID = CCID,  " +
                "      @TenantID = TenantID   " +
                "   FROM  " +
                "      tblPurchase   " +
                "   WHERE  " +
                "      InvId = @InvID   " +
                "      SELECT  " +
                "         @BatchMode = BatchMode   " +
                "      FROM  " +
                "         tblItemMaster   " +
                "      WHERE  " +
                "         ItemID = @ItemId   " +
                "         SELECT  " +
                "            @VchParentID = ParentID   " +
                "         FROM  " +
                "            tblVchType   " +
                "         WHERE  " +
                "            VchTypeID = @VchTypeID IF @Action = 0   " +
                "            BEGIN  " +
                "               IF @VchParentID = 2   " +
                "               BEGIN  " +
                "                  EXEC UspTransStockUpdate @ItemId,  " +
                "                  @BatchCode,  " +
                "                  @BatchUnique,  " +
                "                  @Qty,  " +
                "                  @MRP,  " +
                "                  @CRate,  " +
                "                  @CRate,  " +
                "                  @Prate,  " +
                "                  @Prate,  " +
                "                  @TaxPer,  " +
                "                  @Srate1,  " +
                "                  @Srate2,  " +
                "                  @Srate3,  " +
                "                  @Srate4,  " +
                "                  @Srate5,  " +
                "                  @BatchMode,  " +
                "                  @VchType,  " +
                "                  @VchDate,  " +
                "                  @Expiry,  " +
                "                  'STOCKADD',  " +
                "                  @InvID,  " +
                "                  @VchTypeID,  " +
                "                  @CCID,  " +
                "                  @TenantID,  " +
                "                  @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0   " +
                "                  BEGIN  " +
                "         SET  " +
                "            @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))   " +
                "                  END  " +
                "               END  " +
                "               ELSE IF @VchParentID = 4   " +
                "               BEGIN  " +
                "                     EXEC UspTransStockUpdate @ItemId,  " +
                "                     @BatchCode,  " +
                "                     @BatchUnique,  " +
                "                     @Qty,  " +
                "                     @MRP,  " +
                "                     @CRate,  " +
                "                     @CRate,  " +
                "                     @Prate,  " +
                "                     @Prate,  " +
                "                     @TaxPer,  " +
                "                     @Srate1,  " +
                "                     @Srate2,  " +
                "                     @Srate3,  " +
                "                     @Srate4,  " +
                "                     @Srate5,  " +
                "                     @BatchMode,  " +
                "                     @VchType,  " +
                "                     @VchDate,  " +
                "                     @Expiry,  " +
                "                     'STOCKLESS',  " +
                "                     @InvID,  " +
                "                     @VchTypeID,  " +
                "                     @CCID,  " +
                "                     @TenantID,  " +
                "                     @BarCode_out output   " +
                "   				IF CHARINDEX('@', @BarCode_out) > 0   " +
                "   				BEGIN  " +
                "   					SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))   " +
                "                   END  " +
                "   			END  " +
                "   			ELSE IF @VchParentID = 6   " +
                "               BEGIN  " +
                "                  EXEC UspTransStockUpdate @ItemId,  " +
                "                  @BatchCode,  " +
                "                  @BatchUnique,  " +
                "                  @Qty,  " +
                "                  @MRP,  " +
                "                  @CRate,  " +
                "                  @CRate,  " +
                "                  @Prate,  " +
                "                  @Prate,  " +
                "                  @TaxPer,  " +
                "                  @Srate1,  " +
                "                  @Srate2,  " +
                "                  @Srate3,  " +
                "                  @Srate4,  " +
                "                  @Srate5,  " +
                "                  @BatchMode,  " +
                "                  @VchType,  " +
                "                  @VchDate,  " +
                "                  @Expiry,  " +
                "                  'STOCKADD',  " +
                "                  @InvID,  " +
                "                  @VchTypeID,  " +
                "                  @CCID,  " +
                "                  @TenantID,  " +
                "                  @BarCode_out output   " +
                "   			   IF CHARINDEX('@', @BarCode_out) > 0   " +
                "                  BEGIN  " +
                "   				  SET  " +
                "   					 @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))   " +
                "                  END  " +
                "               END  " +
                "   			ELSE  " +
                "   				BEGIN  " +
                "         SET  " +
                "            @BatchCode = @BarCode_out   " +
                "                     END  " +
                "                     INSERT INTO  " +
                "                        tblPurchaseItem(InvID, ItemId, Qty, Rate, UnitId, Batch, TaxPer, TaxAmount, Discount, MRP, SlNo, Prate, Free, SerialNos, ItemDiscount, BatchCode, iCessOnTax, blnCessOnTax, Expiry, ItemDiscountPer, RateInclusive, ITaxableAmount, INetAmount, CGSTTaxPer, CGSTTaxAmt, SGSTTaxPer, SGSTTaxAmt, IGSTTaxPer, IGSTTaxAmt, iRateDiscPer, iRateDiscount, BatchUnique, blnQtyIN, CRate, Unit, ItemStockID, IcessPercent, IcessAmt, IQtyCompCessPer, IQtyCompCessAmt, StockMRP, BaseCRate, InonTaxableAmount, IAgentCommPercent, BlnDelete, StrOfferDetails, BlnOfferItem, BalQty, GrossAmount, iFloodCessPer, iFloodCessAmt, Srate1, Srate2, Srate3, Srate4, Srate5, Costrate, CostValue, Profit, ProfitPer, DiscMode, Srate1Per, Srate2Per, Srate3Per, Srate4Per, Srate5Per)   " +
                "                     VALUES  " +
                "                        (  " +
                "                           @InvID,  " +
                "                           @ItemId,  " +
                "                           @Qty,  " +
                "                           @Rate,  " +
                "                           @UnitId,  " +
                "                           @Batch,  " +
                "                           @TaxPer,  " +
                "                           @TaxAmount,  " +
                "                           @Discount,  " +
                "                           @MRP,  " +
                "                           @SlNo,  " +
                "                           @Prate,  " +
                "                           @Free,  " +
                "                           @SerialNos,  " +
                "                           @ItemDiscount,  " +
                "                           @BatchCode,  " +
                "                           @iCessOnTax,  " +
                "                           @blnCessOnTax,  " +
                "                           @Expiry,  " +
                "                           @ItemDiscountPer,  " +
                "                           @RateInclusive,  " +
                "                           @ITaxableAmount,  " +
                "                           @INetAmount,  " +
                "                           @CGSTTaxPer,  " +
                "                           @CGSTTaxAmt,  " +
                "                           @SGSTTaxPer,  " +
                "                           @SGSTTaxAmt,  " +
                "                           @IGSTTaxPer,  " +
                "                           @IGSTTaxAmt,  " +
                "                           @iRateDiscPer,  " +
                "                           @iRateDiscount,  " +
                "                           @BarCode_out,  " +
                "                           @blnQtyIN,  " +
                "                           @CRate,  " +
                "                           @Unit,  " +
                "                           @ItemStockID,  " +
                "                           @IcessPercent,  " +
                "                           @IcessAmt,  " +
                "                           @IQtyCompCessPer,  " +
                "                           @IQtyCompCessAmt,  " +
                "                           @StockMRP,  " +
                "                           @BaseCRate,  " +
                "                           @InonTaxableAmount,  " +
                "                           @IAgentCommPercent,  " +
                "                           @BlnDelete,  " +
                "                           @StrOfferDetails,  " +
                "                           @BlnOfferItem,  " +
                "                           @BalQty,  " +
                "                           @GrossAmount,  " +
                "                           @iFloodCessPer,  " +
                "                           @iFloodCessAmt,  " +
                "                           @Srate1,  " +
                "                           @Srate2,  " +
                "                           @Srate3,  " +
                "                           @Srate4,  " +
                "                           @Srate5,  " +
                "                           @Costrate,  " +
                "                           @CostValue,  " +
                "                           @Profit,  " +
                "                           @ProfitPer,  " +
                "                           @DiscMode,  " +
                "                           @Srate1Per,  " +
                "                           @Srate2Per,  " +
                "                           @Srate3Per,  " +
                "                           @Srate4Per,  " +
                "                           @Srate5Per   " +
                "                        )  " +
                "                     SET  " +
                "                        @RetResult = 1;  " +
                "            END  " +
                "            ELSE  " +
                "               IF @Action = 2   " +
                "               BEGIN  " +
                "                  EXEC UspTransStockUpdate @ItemId,  " +
                "                  @BatchCode,  " +
                "                  @BatchUnique,  " +
                "                  @Qty,  " +
                "                  @MRP,  " +
                "                  @CRate,  " +
                "                  @CRate,  " +
                "                  @Prate,  " +
                "                  @Prate,  " +
                "                  @TaxPer,  " +
                "                  @Srate1,  " +
                "                  @Srate2,  " +
                "                  @Srate3,  " +
                "                  @Srate4,  " +
                "                  @Srate5,  " +
                "                  @BatchMode,  " +
                "                  @VchType,  " +
                "                  @VchDate,  " +
                "                  @Expiry,  " +
                "                  'STOCKDEL',  " +
                "                  @InvID,  " +
                "                  @VchTypeID,  " +
                "                  @CCID,  " +
                "                  @TenantID,  " +
                "                  @BarCode_out output   " +
                "                  DELETE  " +
                "                  FROM  " +
                "                     tblPurchaseItem   " +
                "                  WHERE  " +
                "                     InvID = @InvID   " +
                "                  SET  " +
                "                     @RetResult = 0;  " +
                "               END  " +
                "               ELSE  " +
                "                  IF @Action = 3   " +
                "                  BEGIN  " +
                "                     EXEC UspTransStockUpdate @ItemId,  " +
                "                     @BatchCode,  " +
                "                     @BatchUnique,  " +
                "                     @Qty,  " +
                "                     @MRP,  " +
                "                     @CRate,  " +
                "                     @CRate,  " +
                "                     @Prate,  " +
                "                     @Prate,  " +
                "                     @TaxPer,  " +
                "                     @Srate1,  " +
                "                     @Srate2,  " +
                "                     @Srate3,  " +
                "                     @Srate4,  " +
                "                     @Srate5,  " +
                "                     @BatchMode,  " +
                "                     @VchType,  " +
                "                     @VchDate,  " +
                "                     @Expiry,  " +
                "                     'STOCKDEL',  " +
                "                     @InvID,  " +
                "                     @VchTypeID,  " +
                "                     @CCID,  " +
                "                     @TenantID,  " +
                "                     @BarCode_out output   " +
                "                  SET  " +
                "                     @RetResult = 0;  " +
                "                  END  " +
                "                  COMMIT TRANSACTION;  " +
                "   SELECT  " +
                "      @RetResult as SqlSpResult,  " +
                "      @RetID as PID   " +
                "         END  " +
                "         TRY   " +
                "         BEGIN  " +
                "            CATCH ROLLBACK;  " +
                "   SELECT  " +
                "      - 1 as SqlSpResult,  " +
                "      @RetID as PID,  " +
                "      ERROR_NUMBER() AS ErrorNumber,  " +
                "      ERROR_STATE() AS ErrorState,  " +
                "      ERROR_SEVERITY() AS ErrorSeverity,  " +
                "      ERROR_PROCEDURE() AS ErrorProcedure,  " +
                "      ERROR_LINE() AS ErrorLine,  " +
                "      ERROR_MESSAGE() AS ErrorMessage;  " +
                "         END  " +
                "         CATCH;  " +
                "      END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetPurchaseMaster') " +
          "DROP PROCEDURE UspGetPurchaseMaster";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspGetPurchaseMaster]( @InvId NUMERIC (18, 0), @TenantID NUMERIC (18, 0), @VchTypeID NUMERIC (18, 0), @blnPrevNext BIT = 0) AS   " +
                "   BEGIN  " +
                "      declare @PrevVoucherNo int   " +
                "      declare @NextVoucherNo int   " +
                "      DECLARE @InvId_Org INT IF @InvId <> 0   " +
                "      BEGIN  " +
                "         IF @blnPrevNext = 0   " +
                "         BEGIN  " +
                "            SELECT  " +
                "               party,  " +
                "               InvId,  " +
                "               InvNo,  " +
                "               AutoNum,  " +
                "               Prefix,  " +
                "               convert(varchar(10), InvDate, 105) as InvDate,  " +
                "               convert(varchar(10), EffectiveDate, 105) as EffectiveDate,  " +
                "               RefNo,  " +
                "               ReferenceAutoNO,  " +
                "               MOP,  " +
                "               TaxModeID,  " +
                "               CCID,  " +
                "               SalesManID,  " +
                "               AgentID,  " +
                "               MobileNo,  " +
                "               StateID,  " +
                "               GSTType,  " +
                "               PartyAddress,  " +
                "               GrossAmt,  " +
                "               ItemDiscountTotal,  " +
                "               DiscPer,  " +
                "               Discount,  " +
                "               Taxable,  " +
                "               NonTaxable,  " +
                "               TaxAmt,  " +
                "               OtherExpense,  " +
                "               NetAmount,  " +
                "               CashDiscount,  " +
                "               RoundOff,  " +
                "               UserNarration,  " +
                "               BillAmt,  " +
                "               PartyGSTIN,  " +
                "               Isnull(CashDisPer, 0) as CashDisPer,  " +
                "               Isnull(CostFactor, 0) as CostFactor,  " +
                "               LedgerId,  " +
                "               Cancelled,  " +
                "               JsonData   " +
                "            FROM  " +
                "               tblPurchase   " +
                "            WHERE  " +
                "               InvId = @InvId   " +
                "               AND TenantID = @TenantID   " +
                "               AND VchTypeID = @VchTypeID   " +
                "         END  " +
                "         ELSE  " +
                "            BEGIN  " +
                "               SELECT  " +
                "                  @InvId_Org = InvId   " +
                "               FROM  " +
                "                  tblPurchase   " +
                "               WHERE  " +
                "                  InvNo = @InvId   " +
                "                  AND TenantID = @TenantID   " +
                "                  AND VchTypeID = @VchTypeID   " +
                "                  SELECT  " +
                "                     TOP 1 @PrevVoucherNo = InvId   " +
                "                  FROM  " +
                "                     tblPurchase   " +
                "                  WHERE  " +
                "                     InvId < @InvId_Org   " +
                "                     AND VchTypeID = @VchTypeID   " +
                "                  ORDER BY  " +
                "                     InvId DESC   " +
                "                     SELECT  " +
                "                        TOP 1 @NextVoucherNo = InvId   " +
                "                     FROM  " +
                "                        tblPurchase   " +
                "                     WHERE  " +
                "                        InvId > @InvId_Org   " +
                "                        AND VchTypeID = @VchTypeID   " +
                "                     ORDER BY  " +
                "                        InvId ASC   " +
                "                        SELECT  " +
                "                           ISNULL(@PrevVoucherNo, 0) As PrevVoucherNo,  " +
                "                           ISNULL(@NextVoucherNo, 0) As NextVoucherNo   " +
                "            END  " +
                "      END  " +
                "      ELSE  " +
                "         BEGIN  " +
                "            SELECT  " +
                "               InvId,  " +
                "               AutoNum as [Invoice No],  " +
                "               CONVERT(varchar(12), InvDate) as [Invoice Date],  " +
                "               MOP,  " +
                "               Party as [Supplier],  " +
                "               ItemDiscountTotal as [Item Discount],  " +
                "               CashDiscount as [Cash Discount],  " +
                "               Taxable as [Taxable],  " +
                "               TaxAmt as [Tax],  " +
                "               Discount as [Discount],  " +
                "               RoundOff as [RoundOff],  " +
                "               BillAmt as [Bill Amount],  " +
                "               (  " +
                "                  CASE  " +
                "                     WHEN  " +
                "                        ISNULL(Cancelled, 0) = 0   " +
                "                     THEN  " +
                "                        'Active'   " +
                "                     ELSE  " +
                "                        'Cancelled'   " +
                "                  END  " +
                "               )  " +
                "               as [Bill Status]   " +
                "            FROM  " +
                "               tblPurchase   " +
                "            WHERE  " +
                "               TenantID = @TenantID   " +
                "               AND VchTypeID = @VchTypeID   " +
                "            order by  " +
                "               InvID asc   " +
                "         END  " +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdate') " +
         "DROP PROCEDURE UspTransStockUpdate";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspTransStockUpdate]  " +
                "    (  " +
                "    	@ItemID			NUMERIC(18,0),  " +
                "    	@BatchCode		VARCHAR(50),  " +
                "    	@BatchUniq		VARCHAR(50),  " +
                "    	@Qty			NUMERIC(18,5),  " +
                "    	@MRP			NUMERIC(18,5),  " +
                "    	@CostRateInc	NUMERIC(18,5),  " +
                "    	@CostRateExcl	NUMERIC(18,5),  " +
                "    	@PRateExcl		NUMERIC(18,5),  " +
                "    	@PrateInc		NUMERIC(18,5),  " +
                "    	@TaxPer			NUMERIC(18,5),  " +
                "    	@SRate1			NUMERIC(18,5),  " +
                "    	@SRate2			NUMERIC(18,5),  " +
                "    	@SRate3			NUMERIC(18,5),  " +
                "    	@SRate4			NUMERIC(18,5),  " +
                "    	@SRate5			NUMERIC(18,5),  " +
                "    	@BatchMode		INT,  " +
                "    	@VchType		VARCHAR(100),  " +
                "    	@VchDate		DATETIME,  " +
                "    	@ExpDt			DATETIME,  " +
                "    	@Action	 	    VARCHAR(20),  " +
                "    	@RefID			NUMERIC(18,0),  " +
                "    	@VchTypeID		NUMERIC(18,0),  " +
                "    	@CCID			NUMERIC(18,0),  " +
                "    	@TenantID		NUMERIC(18,0),  " +
                "    	@BarCode_out	VARCHAR(50) OUTPUT  " +
                "    )  " +
                "    AS  " +
                "    BEGIN  " +
                "    	DECLARE @BatchID		NUMERIC(18,0)  " +
                "    	DECLARE @StockID		NUMERIC(18,0)  " +
                "    	DECLARE @LastInvDt		DATETIME = Getdate()  " +
                "    	DECLARE @STOCKHISID		NUMERIC(18,0)  " +
                "    	DECLARE @PRFXBATCH		VARCHAR(10)  " +
                "    	DECLARE @Stock		   NUMERIC(18,5)  " +
                "    	DECLARE @INVID          NUMERIC(18,0)  " +
                "    	DECLARE @BarCode		VARCHAR(50)  " +
                "    	DECLARE @BarUniq		VARCHAR(100)  " +
                "    	DECLARE @CalcQOH		NUMERIC(18,5)  " +
                "    	DECLARE @BLNADVANCED	INT  " +
                "    	DECLARE @blnExpiry		BIT  " +
                "    	DECLARE @LessQty		NUMERIC(18,5)  " +
                "    	SET @BarCode = @BatchCode  " +
                "    	SELECT @StockID = MAX(ISNULL(StockID,0)) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "    	SELECT @BatchID = MAX(ISNULL(BatchID,0)) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "    	SELECT @STOCKHISID = MAX(ISNULL(STOCKHISID,0)) + 1 FROM tblStockHistory WHERE TenantID = @TenantID  " +
                "    	SELECT @BLNADVANCED = ISNULl(ValueName,0) FROM [tblAppSettings] WHERE UPPER(LTRIM(RTRIM(KeyName))) = 'BLNADVANCED'   " +
                "    	SELECT @Stock = ISNULL(QOH,0)FROM tblStock WHERE ItemID= @ItemID AND BatchCode = @BatchCode AND TenantID = @TenantID  " +
                "    	SELECT @blnExpiry = ISNULL(blnExpiry,0) FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "    	SET @LessQty = -1  " +
                "    	IF @StockID = 0  " +
                "    	BEGIN  " +
                "    		SET @StockID = 1  " +
                "    	END  " +
                "    	IF @STOCKHISID = 0  " +
                "    	BEGIN  " +
                "    		SET @STOCKHISID = 1  " +
                "    	END  " +
                "    	IF @BatchID = 0  " +
                "    	BEGIN  " +
                "    		SET @BatchID = 1  " +
                "    	END  " +
                "    	IF @Action = 'STOCKADD'  " +
                "    	BEGIN  " +
                "    		IF @BatchCode = '<Auto Barcode>'  " +
                "    		BEGIN  " +
                "    			Declare @Prefix VARCHAR(50)  " +
                "    			Declare @BatchPrefix VARCHAR(50)  " +
                "    			IF @BLNADVANCED = 1  " +
                "    			BEGIN  " +
                "    				Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "    				set @BatchPrefix= (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "    				IF(@BatchPrefix='<YEARMONTH>')  " +
                "    				BEGIN  " +
                "    					SELECT @Prefix=(Select [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix))  " +
                "    					SET @BatchCode = @Prefix + CONVERT(VARCHAR,@BatchID)  " +
                "    				END  " +
                "    				ELSE  " +
                "    				BEGIN  " +
                "    					Select @BatchPrefix = ValueName from tblAppSettings where KeyName='STRBATCODEPREFIXSUFFIX'  " +
                "    					set @BatchPrefix = (SELECT PARSENAME(REPLACE(@BatchPrefix , 'ƒ', ''),1))  " +
                "    					SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "    					IF @BatchPrefix <> ''  " +
                "    					BEGIN  " +
                "    						SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchID)  " +
                "    					END  " +
                "    				END  " +
                "    			END  " +
                "    			ELSE  " +
                "    			BEGIN  " +
                "    				SELECT @BatchCode = ISNULL(MAX(BatchID),0) + 1 FROM tblStock WHERE TenantID = @TenantID  " +
                "    			END  " +
                "    			SET @BarCode =  @BatchCode  " +
                "    			IF @blnExpiry = 1  " +
                "    			BEGIN  " +
                "    				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "    				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "    			END  " +
                "    			ELSE  " +
                "    			BEGIN  " +
                "    				SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))   " +
                "    				SET @BarUniq = @BarCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))   " +
                "    			END  " +
                "    			IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)  " +
                "    			BEGIN  " +
                "    				IF @VchTypeID <> 0  " +
                "    				BEGIN  " +
                "    					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "    					INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    					VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    				END  " +
                "    			END  " +
                "    			ELSE  " +
                "    			BEGIN  " +
                "    				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID  AND CCID = @CCID)  " +
                "    				BEGIN  " +
                "    					IF @VchTypeID <> 0  " +
                "    					BEGIN  " +
                "    						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "    						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    					END  " +
                "    				END  " +
                "    				ELSE  " +
                "    				BEGIN  " +
                "    					IF @VchTypeID <> 0  " +
                "    					BEGIN  " +
                "    						EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "    						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    					END  " +
                "    				END  " +
                "    			END  " +
                "    		END  " +
                "    		ELSE  " +
                "    		BEGIN  " +
                "    			IF @BatchMode = 0   " +
                "    			BEGIN  " +
                "    				SELECT @BatchCode = ItemCode FROM tblItemMaster WHERE ItemID = @ItemID  " +
                "    				SET @BarCode = @BatchCode  " +
                "    				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "    				BEGIN  " +
                "    					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "    					IF @VchTypeID <> 0  " +
                "    					BEGIN  " +
                "    						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    					END  " +
                "    				END  " +
                "    				ELSE  " +
                "    				BEGIN  " +
                "    					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "    					IF @VchTypeID <> 0  " +
                "    					BEGIN  " +
                "    						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    					END  " +
                "    				END  " +
                "    			END  " +
                "    			ELSE  " +
                "    			BEGIN  " +
                "    				IF CHARINDEX('@',@BatchUniq) = 0  " +
                "    				BEGIN  " +
                "    					IF @blnExpiry = 1  " +
                "    					BEGIN  " +
                "    						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "    					END  " +
                "    					ELSE  " +
                "    					BEGIN  " +
                "    						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "    					END  " +
                "    				END  " +
                "    				ELSE  " +
                "    				BEGIN  " +
                "    					IF @blnExpiry = 1  " +
                "    					BEGIN  " +
                "    						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP)) + '@' + REPLACE(CONVERT(VARCHAR(10),FORMAT(@ExpDt,'dd-MM-yy')),'-','')  " +
                "    					END  " +
                "    					ELSE  " +
                "    					BEGIN  " +
                "    						SET @BatchUniq = @BatchCode + '@' + CONVERT(VARCHAR(22),CONVERT(NUMERIC(18,2),@MRP))  " +
                "    					END  " +
                "    				END  " +
                "    				IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID AND MRP=@MRP AND FORMAT(ExpiryDate,'dd-MM-yy') = FORMAT(@ExpDt,'dd-MM-yy'))   " +
                "    				BEGIN  " +
                "    					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "    					IF @VchTypeID <> 0  " +
                "    					BEGIN  " +
                "    						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    					END  " +
                "    				END  " +
                "    				ELSE  " +
                "    				BEGIN  " +
                "    					EXEC UspStockInsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode   " +
                "    					IF @VchTypeID <> 0  " +
                "    					BEGIN  " +
                "    						INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    						VALUES(@VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    					END  " +
                "    				END  " +
                "    			END  " +
                "    		END  " +
                "    		SET @BatchCode = @BarCode  " +
                "    	END  " +
                "    	IF @Action = 'STOCKLESS'  " +
                "    	BEGIN  " +
                "    		IF EXISTS(SELECT * FROM tblStock WHERE ItemID = @ItemID AND BatchCode = @BarCode AND TenantID = @TenantID AND BatchUnique = @BatchUniq AND CCID = @CCID)   " +
                "    		BEGIN  " +
                "    			IF @VchTypeID <> 0  " +
                "    			BEGIN  " +
                "    				SET @LessQty = @LessQty * @Qty  " +
                "    				EXEC UspStockInsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@LessQty ,@LastInvDt,'',NULL,1,@ItemID,@BatchMode   " +
                "    				INSERT INTO tblStockHistory(VchType,VchDate,RefId,ItemID,QtyIn,QtyOut,BatchCode,BatchUnique,Expiry,MRP,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,SRate1,SRate2,SRate3,SRate4,SRate5,VchTypeID,CCID,STOCKHISID,TenantID,StockID)  " +
                "    				VALUES(@VchType,@VchDate,@RefID,@ItemID,0,@Qty,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID)  " +
                "    			END  " +
                "    		END  " +
                "    	END  " +
                "    	IF @Action = 'STOCKDEL'  " +
                "    	BEGIN  " +
                "    		SELECT @CalcQOH = QOH FROM tblStock WHERE ItemID = @ItemID AND CCID=@CCID AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "    		UPDATE tblStock SET QOH = QOH - @CalcQOH WHERE ItemID = @ItemID AND CCID=@CCID AND BatchUnique=@BatchUniq AND TenantID=@TenantID  " +
                "    		DELETE FROM tblStockHistory WHERE RefId = @RefID AND CCID = @CCID AND TenantID = @TenantID  " +
                "    	END  " +
                "    	SET @BarCode_out = @BatchUniq  " +
                "    	SELECT @BarCode_out  " +
                "    END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockHistory') " +
         "DROP PROCEDURE UspGetStockHistory";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspGetStockHistory] ( @ItemID Numeric(18, 0), @VchTypeID Numeric(18, 0) = NULL, @BatchUnique varchar(MAX) = NULL, @CostCentreID Numeric(18, 0) = NULL, @FromDate Datetime, @ToDate Datetime, @TenantID Numeric(18, 0) ) AS   " +
                "   BEGIN  " +
                "      DECLARE @TempTable Table ( VoucherType varchar(500), InvoiceNo Varchar(50), VoucherDate datetime, Batch varchar(500), QtyIn Numeric(18, 0), QtyOut Numeric(18, 0), PRate Numeric(18, 4), SRate Numeric(18, 4), Vchtypeid Numeric(18, 0), StockID Numeric(18, 0), Unit varchar(500), ItemID Numeric(18, 0), ItemCode varchar(500), CCID Numeric(18, 0) )   " +
                "      INSERT INTO  " +
                "         @TempTable   " +
                "         SELECT  " +
                "            TSH.vchtype,  " +
                "            Invno,  " +
                "            VchDate,  " +
                "            TSH.batchUnique,  " +
                "            round(isnull(QTYIN, 0), 3),  " +
                "            round(isnull(QTYOUT, 0), 3),  " +
                "            Round(TSH.PRateExcl, 2),  " +
                "            Round(TSH.SRate1, 2),  " +
                "            VIA.Vchtypeid,  " +
                "            TS.StockID,  " +
                "            TU.UnitShortName,  " +
                "            TSH.ItemID,  " +
                "            ItemCode,  " +
                "            TSH.CCID   " +
                "         FROM  " +
                "            tblStockHistory TSH   " +
                "            LEFT JOIN  " +
                "               tblstock TS   " +
                "               ON TSH.BatchUnique = TS.batchUnique   " +
                "            LEFT JOIN  " +
                "               VWitemAnalysis VIA   " +
                "               ON TSH.RefId = VIA.Invid   " +
                "            LEFT JOIN  " +
                "               tblItemMaster IM   " +
                "               ON TSH.ItemID = IM.ItemID   " +
                "            LEFT JOIN  " +
                "               tblUnit TU   " +
                "               ON IM.UNITID = TU.UnitID   " +
                "         WHERE  " +
                "            TSH.TenantID = @TenantID   " +
                "            AND TSH.ItemID = @ItemID   " +
                "            AND convert(datetime, VchDate, 106) >= @FromDate   " +
                "            AND convert(datetime, VchDate, 106) <= @ToDate   " +
                "            AND VIA.VchType IS NOT NULL   " +
                "   		 IF @VchTypeID <> 0   " +
                "            BEGIN  " +
                "               DELETE  " +
                "               FROM  " +
                "                  @TempTable   " +
                "               WHERE  " +
                "                  VchTypeID <> @VchTypeID   " +
                "            END  " +
                "            IF @BatchUnique <> 0   " +
                "            BEGIN  " +
                "               DELETE  " +
                "               FROM  " +
                "                  @TempTable   " +
                "               WHERE  " +
                "                  StockID <> @BatchUnique   " +
                "            END  " +
                "            IF @CostCentreID <> 0   " +
                "            BEGIN  " +
                "               DELETE  " +
                "               FROM  " +
                "                  @TempTable   " +
                "               WHERE  " +
                "                  CCID <> @CostCentreID   " +
                "            END  " +
                "            SELECT  " +
                "               VoucherType AS[Voucher Type],  " +
                "               InvoiceNo AS[Invoice No],  " +
                "               CONVERT(VARCHAR(12), FORMAT(VoucherDate, 'dd-MMM-yyyy')) As[Voucher Date],  " +
                "               Batch AS[Batch],  " +
                "               QtyIn AS[Qty In],  " +
                "               QtyOut AS[Qty Out],  " +
                "               Unit AS[Unit],  " +
                "               PRate AS[P.Rate],  " +
                "               SRate AS[S.Rate]   " +
                "            FROM  " +
                "               @TempTable   " +
                "            Order by  " +
                "               VoucherDate   " +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF NOT EXISTS(SELECT SizeID FROM tblsize WHERE SizeID = 1) " +
           "INSERT INTO tblsize(SizeID, SizeName, SizeNameShort, SortOrder, TenantID) " +
           "VALUES(1, '<None>', '<None>', 1, 1) ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "UPDATE tblAgent SET LID = NULL where AgentID = 1 and LID = -2 ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblAgent]  WITH CHECK ADD CONSTRAINT[FK_Agent_Ledger] FOREIGN KEY([LID]) " +
            "REFERENCES[dbo].[tblLedger]([LID]) " +
            "ALTER TABLE[dbo].[tblAgent] CHECK CONSTRAINT[FK_Agent_Ledger] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "UPDATE tblAgent SET LID = 0 where AgentID = 1 and LID IS NULL";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblAgent ADD AreaID Numeric(18, 0) ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblAgent]  WITH CHECK ADD CONSTRAINT[FK_Agent_Area] FOREIGN KEY([AreaID]) " +
            "REFERENCES[dbo].[tblArea]([AreaID]) " +
            "ALTER TABLE[dbo].[tblAgent] CHECK CONSTRAINT[FK_Agent_Area] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblLedger]  WITH CHECK ADD CONSTRAINT[FK_Ledger_State] FOREIGN KEY([StateID]) " +
            "REFERENCES[dbo].[tblStates]([StateId]) " +
            "ALTER TABLE[dbo].[tblLedger] CHECK CONSTRAINT[FK_Ledger_State] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblStates]  WITH CHECK ADD CONSTRAINT[FK_States_Country] FOREIGN KEY([CountryID]) " +
            "REFERENCES[dbo].[tblCountry]([CountryID]) " +
            "ALTER TABLE[dbo].[tblStates] CHECK CONSTRAINT[FK_States_Country] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblUserMaster]  WITH CHECK ADD CONSTRAINT[FK_UserMaster_Ledger] FOREIGN KEY([UserLedgerID]) " +
            "REFERENCES[dbo].[tblLedger]([LID]) " +
            "ALTER TABLE[dbo].[tblUserMaster] CHECK CONSTRAINT[FK_UserMaster_Ledger] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblUserMaster ALTER COLUMN GroupID Numeric(18, 0) ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblUserMaster]  WITH CHECK ADD CONSTRAINT[FK_UserMaster_UserGroup] FOREIGN KEY([GroupID]) " +
            "REFERENCES[dbo].[tblUserGroupMaster]([ID]) " +
            "ALTER TABLE[dbo].[tblUserMaster] CHECK CONSTRAINT[FK_UserMaster_UserGroup] ";
            Comm.fnExecuteNonQuery(sQuery, false);



            sQuery = "ALTER TABLE[dbo].[tblLedger]  WITH CHECK ADD CONSTRAINT[FK_Ledger_Area] FOREIGN KEY([AreaID]) " +
                    "REFERENCES[dbo].[tblArea]([AreaID]) " +
                    "ALTER TABLE[dbo].[tblLedger] CHECK CONSTRAINT[FK_Ledger_Area] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblLedger]  WITH CHECK ADD CONSTRAINT[FK_Ledger_Agent] FOREIGN KEY([AgentID]) " +
                     "REFERENCES[dbo].[tblAgent]([AgentID]) " +
                     "ALTER TABLE[dbo].[tblLedger] CHECK CONSTRAINT[FK_Ledger_Agent] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblUserMaster]  WITH CHECK ADD CONSTRAINT[FK_UserMaster_CostCentre] FOREIGN KEY([SelectedCCID]) " +
                     "REFERENCES[dbo].[tblCostCentre]([CCID]) " +
                     "ALTER TABLE[dbo].[tblUserMaster] CHECK CONSTRAINT[FK_UserMaster_CostCentre] ";
            Comm.fnExecuteNonQuery(sQuery, false);




            sQuery = " ALTER TABLE[dbo].[tblItemMaster]  WITH CHECK ADD CONSTRAINT[FK_ItemMaster_Brand] FOREIGN KEY([BrandID]) " +
                   "REFERENCES [dbo].[tblBrand] ([brandID]) " +
                   "ALTER TABLE [dbo].[tblItemMaster] CHECK CONSTRAINT [FK_ItemMaster_Brand] ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "ALTER TABLE [dbo].[tblItemMaster]  WITH CHECK ADD  CONSTRAINT [FK_ItemMaster_Categories] FOREIGN KEY([CategoryID]) " +
                     "REFERENCES [dbo].[tblCategories] ([CategoryID]) " +
                     "ALTER TABLE [dbo].[tblItemMaster] CHECK CONSTRAINT [FK_ItemMaster_Categories] ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "ALTER TABLE [dbo].[tblItemMaster]  WITH CHECK ADD  CONSTRAINT [FK_ItemMaster_Department] FOREIGN KEY([DepartmentID]) " +
                     "REFERENCES [dbo].[tblDepartment] ([DepartmentID]) " +
                     "ALTER TABLE [dbo].[tblItemMaster] CHECK CONSTRAINT [FK_ItemMaster_Department] ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "ALTER TABLE [dbo].[tblItemMaster]  WITH CHECK ADD  CONSTRAINT [FK_ItemMaster_DiscountGroup] FOREIGN KEY([DGroupID]) " +
                     "REFERENCES [dbo].[tblDiscountGroup] ([DiscountGroupID]) " +
                     "ALTER TABLE [dbo].[tblItemMaster] CHECK CONSTRAINT [FK_ItemMaster_DiscountGroup] ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "ALTER TABLE [dbo].[tblItemMaster]  WITH CHECK ADD  CONSTRAINT [FK_ItemMaster_Unit] FOREIGN KEY([UNITID]) " +
                     "REFERENCES [dbo].[tblUnit] ([UnitID]) " +
                     "ALTER TABLE [dbo].[tblItemMaster] CHECK CONSTRAINT [FK_ItemMaster_Unit] ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "ALTER TABLE [dbo].[tblItemMaster]  WITH CHECK ADD  CONSTRAINT [FK_tblItemMaster_tblManufacturer] FOREIGN KEY([MNFID]) " +
                     "REFERENCES [dbo].[tblManufacturer] ([MnfID]) " +
                     "ALTER TABLE [dbo].[tblItemMaster] CHECK CONSTRAINT [FK_tblItemMaster_tblManufacturer] ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE[dbo].[tblStock]  WITH CHECK ADD CONSTRAINT[FK_tblStock_tblItemMaster] FOREIGN KEY([ItemID]) " +
                     "REFERENCES [dbo].[tblItemMaster] ([ItemID]) " +
                     "ALTER TABLE [dbo].[tblStock] CHECK CONSTRAINT [FK_tblStock_tblItemMaster] ";
            Comm.fnExecuteNonQuery(sQuery, false);


            sQuery = "UPDATE tblAgent SET Area = 'DEFAULT', AreaID = 1 where Area = 'Cherthala' and AgentID = 1 ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspAgentInsert') " +
             "DROP PROCEDURE UspAgentInsert";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "CREATE PROCEDURE [dbo].[UspAgentInsert] " +
            "( " +
            "@AgentID    NUMERIC  (18,0), " +
            "@AgentCode    VARCHAR  (50), " +
            "@AgentName    VARCHAR  (50), " +
            "@Area    VARCHAR  (50), " +
            "@Commission    FLOAT, " +
            "@blnPOstAccounts    NUMERIC  (18,0), " +
            "@ADDRESS    VARCHAR  (500), " +
            "@LOCATION    VARCHAR  (500), " +
            "@PHONE    VARCHAR  (500), " +
            "@WEBSITE    VARCHAR  (500), " +
            "@EMAIL    VARCHAR  (500), " +
            "@BLNROOMRENT    NUMERIC  (18,0), " +
            "@BLNSERVICES    NUMERIC  (18,0), " +
            "@blnItemwiseCommission    INT, " +
            "@AgentDiscount    FLOAT, " +
            "@LID    NUMERIC  (18,0), " +
            "@SystemName    VARCHAR  (50), " +
            "@UserID    NUMERIC  (18,0), " +
            "@LastUpdateDate    DATETIME, " +
            "@LastUpdateTime    DATETIME, " +
            "@TenantID   NUMERIC  (18,0), " +
            "@AreaID   NUMERIC  (18,0), " +
            "@Action             INT=0 " +
            ") " +
            "AS " +
            "BEGIN " +
            "DECLARE @RetResult      INT " +
            "DECLARE @TransType		CHAR(1) " +
            "BEGIN TRY " +
            "BEGIN TRANSACTION; " +
            "IF @LID = -2 " +
            "set @LID = null " +
            "IF @Action = 0 " +
            "BEGIN " +
             "INSERT INTO tblAgent(AgentID,AgentCode,AgentName,Area,Commission,blnPOstAccounts,ADDRESS,LOCATION,PHONE,WEBSITE,EMAIL,BLNROOMRENT,BLNSERVICES,blnItemwiseCommission,AgentDiscount,LID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID,AreaID) " +
             "VALUES(@AgentID,@AgentCode,@AgentName,@Area,@Commission,@blnPOstAccounts,@ADDRESS,@LOCATION,@PHONE,@WEBSITE,@EMAIL,@BLNROOMRENT,@BLNSERVICES,@blnItemwiseCommission,@AgentDiscount,@LID,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID,@AreaID) " +
            "SET @RetResult = 1; " +
            "SET @TransType = 'S'; " +
            "END " +
            "IF @Action = 1 " +
             "BEGIN " +
             "UPDATE tblAgent SET AgentCode=@AgentCode,AgentName=@AgentName,Area=@Area,Commission=@Commission,blnPOstAccounts=@blnPOstAccounts,ADDRESS=@ADDRESS,LOCATION=@LOCATION,PHONE=@PHONE,WEBSITE=@WEBSITE,EMAIL=@EMAIL,BLNROOMRENT=@BLNROOMRENT,BLNSERVICES=@BLNSERVICES,blnItemwiseCommission=@blnItemwiseCommission,AgentDiscount=@AgentDiscount,LID=@LID,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime,AreaID=@AreaID " +
             "WHERE AgentID=@AgentID AND TenantID=@TenantID " +
            "SET @RetResult = 1; " +
            "SET @TransType = 'E'; " +
            "END " +
            "IF @Action = 2 " +
            "BEGIN " +
             "DELETE FROM tblAgent WHERE AgentID=@AgentID AND TenantID=@TenantID " +
            "SET @RetResult = 0; " +
            "SET @TransType = 'D'; " +
            "END " +
            "COMMIT TRANSACTION; " +
            "SELECT @RetResult as SqlSpResult,@AgentID as TransID,@TransType as TransactType " +
            "END TRY " +
            "BEGIN CATCH " +
            "ROLLBACK; " +
            "SELECT " +
            "- 1 as SqlSpResult, " +
            "ERROR_NUMBER() AS ErrorNumber, " +
            "ERROR_STATE() AS ErrorState, " +
            "ERROR_SEVERITY() AS ErrorSeverity, " +
            "ERROR_PROCEDURE() AS ErrorProcedure, " +
            "ERROR_LINE() AS ErrorLine, " +
            "ERROR_MESSAGE() AS ErrorMessage; " +
            "END CATCH; " +
            "END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetAgent') " +
           "DROP PROCEDURE UspGetAgent ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "CREATE PROCEDURE [dbo].[UspGetAgent] " +
            "( " +
            "@AgentID    NUMERIC   (18,0), " +
            "@TenantID     NUMERIC   (18,0) " +
            ") " +
            "AS " +
            "BEGIN " +
            "IF @AgentID <> 0  " +
             "BEGIN " +
             "SELECT TA.AgentID,AgentCode,AgentName,TA.Area,Commission,blnPOstAccounts,TA.ADDRESS,LOCATION,TA.PHONE,WEBSITE,TA.EMAIL,BLNROOMRENT,BLNSERVICES,blnItemwiseCommission,AgentDiscount,ISNULL(TA.LID,-2) as LID,TA.SystemName,TA.UserID,TA.LastUpdateDate,TA.LastUpdateTime,TA.TenantID,LedgerName,TA.AreaID FROM tblAgent TA " +
             "Left Join tblLedger TL ON TL.LID = TA.LID WHERE TA.AgentID = @AgentID AND TA.TenantID = @TenantID ORDER BY AgentCode ASC " +
            "END " +
            "ELSE " +
            "BEGIN " +
            "SELECT AgentID,AgentCode as [Agent Code],AgentName as [Agent Name],Area as [Area],PHONE as [Phone],AgentDiscount as [Discount %],Commission as [Commission %] " +
            "FROM tblAgent WHERE TenantID= @TenantID  ORDER BY AgentCode ASC " +
            "END " +
            "END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspAreaInsert') " +
            "DROP PROCEDURE UspAreaInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "CREATE PROCEDURE [dbo].[UspAreaInsert] " +
            "( " +
            "@AreaID    NUMERIC  (18,0), " +
            "@Area    VARCHAR  (50), " +
            "@Remarks    VARCHAR  (50), " +
            "@ParentID    VARCHAR  (100), " +
            "@HID    VARCHAR  (100), " +
            "@SystemName    VARCHAR  (50), " +
            "@UserID    NUMERIC  (18,0), " +
            "@LastUpdateDate    DATETIME, " +
            "@LastUpdateTime    DATETIME, " +
            "@TenantID   NUMERIC  (18,0), " +
            "@Action             INT=0 " +
            ") " +
            "AS " +
            "BEGIN " +
            "DECLARE @RetResult      INT " +
            "BEGIN TRY " +
            "BEGIN TRANSACTION; " +
            "IF @Action = 0 " +
            "BEGIN " +
            "INSERT INTO tblArea(AreaID,Area,Remarks,ParentID,HID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID) " +
            "VALUES(@AreaID,@Area,@Remarks,@ParentID,@HID,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID) " +
            "SET @RetResult = 1; " +
            "END " +
            "IF @Action = 1 " +
            "BEGIN " +
            "UPDATE tblArea SET Area=@Area,Remarks=@Remarks,ParentID=@ParentID,HID=@HID,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime " +
            "WHERE AreaID=@AreaID AND TenantID=@TenantID " +
            "SET @RetResult = 1; " +
            "END " +
            "IF @Action = 2 " +
            "BEGIN " +
            "DELETE FROM tblArea WHERE AreaID=@AreaID AND TenantID=@TenantID " +
            "SET @RetResult = 0; " +
            "END " +
            "COMMIT TRANSACTION; " +
            "SELECT @RetResult as SqlSpResult " +
            "END TRY " +
            "BEGIN CATCH " +
            "ROLLBACK; " +
            "SELECT " +
            "- 1 as SqlSpResult, " +
            "ERROR_NUMBER() AS ErrorNumber, " +
            "ERROR_STATE() AS ErrorState, " +
            "ERROR_SEVERITY() AS ErrorSeverity, " +
            "ERROR_PROCEDURE() AS ErrorProcedure, " +
            "ERROR_LINE() AS ErrorLine, " +
            "ERROR_MESSAGE() AS ErrorMessage; " +
            "END CATCH; " +
            "END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockReport') " +
           "DROP PROCEDURE UspGetStockReport ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE[dbo].[UspGetStockReport] " +
            "(" +
            "@ToDate Datetime, " +
            "@TenantID Numeric(18, 0) " +
            " )  " +
            "AS " +
            " BEGIN " +
            "Declare @CurrencyDecimal Numeric(18, 0) = (select ValueName from tblAppSettings where KeyName = 'CurrencyDecimals') " +
            " Declare @QtyDecimal Numeric(18, 0) = (select ValueName from tblAppSettings where KeyName = 'QtyDecimalFormat') " +
            "SELECT " +
            " SH.vchtype AS[Voucher Type], " +
            "Invno AS[Invoice No],   " +
            "VchDate As[Voucher Date], " +
            "SH.batchUnique AS[Batch], " +
            "round(isnull(QTYIN, 0), 2) AS[Qty In],   " +
            "round(isnull(QTYOUT, 0), 2) AS[Qty Out],  " +
            "U.UnitShortName AS[Unit], " +
            " Round(SH.PRateExcl, 0) AS[P.Rate], " +
            "Round(SH.SRate1, 0) AS[S.Rate] " +
            "FROM " +
            "tblStockHistory SH " +
            "LEFT JOIN tblstock S ON SH.BatchUnique = S.batchUnique " +
            "LEFT JOIN  VWitemAnalysis VwIA  ON SH.RefId = VwIA.Invid " +
            "LEFT JOIN  tblItemMaster IM    ON SH.ItemID = IM.ItemID " +
            "LEFT JOIN  tblUnit U          ON IM.UNITID = U.UnitID " +
            "WHERE SH.TenantID = @TenantID " +
            "AND convert(datetime, VchDate, 106) <= @ToDate " +
            "AND VwIA.VchType IS NOT NULL " +
            "END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetPurchaseMaster') " +
            "DROP PROCEDURE UspGetPurchaseMaster ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspGetPurchaseMaster]( @InvId NUMERIC (18, 0), @TenantID NUMERIC (18, 0), @VchTypeID NUMERIC (18, 0), @blnPrevNext BIT = 0) AS   " +
                "   BEGIN  " +
                "      declare @PrevVoucherNo int   " +
                "      declare @NextVoucherNo int   " +
                "      DECLARE @InvId_Org INT IF @InvId <> 0   " +
                "      BEGIN  " +
                "         IF @blnPrevNext = 0   " +
                "         BEGIN  " +
                "            SELECT  " +
                "               party,  " +
                "               InvId,  " +
                "               InvNo,  " +
                "               AutoNum,  " +
                "               Prefix,  " +
                "               convert(varchar(10), InvDate, 105) as InvDate,  " +
                "               convert(varchar(10), EffectiveDate, 105) as EffectiveDate,  " +
                "               RefNo,  " +
                "               ReferenceAutoNO,  " +
                "               MOP,  " +
                "               TaxModeID,  " +
                "               CCID,  " +
                "               SalesManID,  " +
                "               AgentID,  " +
                "               MobileNo,  " +
                "               StateID,  " +
                "               GSTType,  " +
                "               PartyAddress,  " +
                "               GrossAmt,  " +
                "               ItemDiscountTotal,  " +
                "               DiscPer,  " +
                "               Discount,  " +
                "               Taxable,  " +
                "               NonTaxable,  " +
                "               TaxAmt,  " +
                "               OtherExpense,  " +
                "               NetAmount,  " +
                "               CashDiscount,  " +
                "               RoundOff,  " +
                "               UserNarration,  " +
                "               BillAmt,  " +
                "               PartyGSTIN,  " +
                "               Isnull(CashDisPer, 0) as CashDisPer,  " +
                "               Isnull(CostFactor, 0) as CostFactor,  " +
                "               LedgerId,  " +
                "               Cancelled,  " +
                "               JsonData   " +
                "            FROM  " +
                "               tblPurchase   " +
                "            WHERE  " +
                "               InvId = @InvId   " +
                "               AND TenantID = @TenantID   " +
                "               AND VchTypeID = @VchTypeID   " +
                "         END  " +
                "         ELSE  " +
                "            BEGIN  " +
                "               SELECT  " +
                "                  @InvId_Org = InvId   " +
                "               FROM  " +
                "                  tblPurchase   " +
                "               WHERE  " +
                "                  InvNo = @InvId   " +
                "                  AND TenantID = @TenantID   " +
                "                  AND VchTypeID = @VchTypeID   " +
                "                  SELECT  " +
                "                     TOP 1 @PrevVoucherNo = InvId   " +
                "                  FROM  " +
                "                     tblPurchase   " +
                "                  WHERE  " +
                "                     InvId < @InvId_Org   " +
                "                     AND VchTypeID = @VchTypeID   " +
                "                  ORDER BY  " +
                "                     InvId DESC   " +
                "                     SELECT  " +
                "                        TOP 1 @NextVoucherNo = InvId   " +
                "                     FROM  " +
                "                        tblPurchase   " +
                "                     WHERE  " +
                "                        InvId > @InvId_Org   " +
                "                        AND VchTypeID = @VchTypeID   " +
                "                     ORDER BY  " +
                "                        InvId ASC   " +
                "                        SELECT  " +
                "                           ISNULL(@PrevVoucherNo, 0) As PrevVoucherNo,  " +
                "                           ISNULL(@NextVoucherNo, 0) As NextVoucherNo   " +
                "            END  " +
                "      END  " +
                "      ELSE  " +
                "         BEGIN  " +
                "            SELECT  " +
                "               InvId,  " +
                "               InvNo as [Invoice No],  " +
                "               CONVERT(varchar(12), InvDate) as [Invoice Date],  " +
                "   			ISNULL(RefNo,'') + CONVERT(VARCHAR,ReferenceAutoNO) as [Reference No],  " +
                "               MOP,  " +
                "               Party as [Supplier],  " +
                "   			MobileNo as [Supplier Contact],  " +
                "               RoundOff as [RoundOff],  " +
                "               BillAmt as [Bill Amount],  " +
                "               (  " +
                "                  CASE  " +
                "                     WHEN  " +
                "                        ISNULL(Cancelled, 0) = 0   " +
                "                     THEN  " +
                "                        'Active'   " +
                "                     ELSE  " +
                "                        'Cancelled'   " +
                "                  END  " +
                "               )  " +
                "               as [Bill Status]   " +
                "            FROM  " +
                "               tblPurchase   " +
                "            WHERE  " +
                "               TenantID = @TenantID   " +
                "               AND VchTypeID = @VchTypeID   " +
                "            order by  " +
                "               InvID asc   " +
                "         END  " +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblVchType ADD DEFTAXINCLUSIVEID int ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblVchType ADD BLNLOCKTAXINCLUSIVE int";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblItemMaster ADD DefaultExpInDays Numeric(18,0) ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspVchTypeInsert')" +
             "DROP PROCEDURE UspVchTypeInsert";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspVchTypeInsert] " +
            " ( " +
                 " @VchTypeID    NUMERIC(18, 0), " +
                "  @VchType    VARCHAR(50), " +
                 " @ShortKey    VARCHAR(50), " +
                "  @EasyKey    VARCHAR(50), " +
                 " @SortOrder    NUMERIC(18, 0), " +
                 " @ActiveStatus    NUMERIC(18, 0), " +
                "  @ParentID    NUMERIC(18, 0), " +
                 " @Description    VARCHAR(250), " +
                 " @numberingCode    NUMERIC(18, 0), " +
                 " @Prefix    VARCHAR(10), " +
                 " @Sufix    VARCHAR(10), " +
                 " @ItemClassIDS    VARCHAR(100), " +
                "  @CreditGroupIDs    VARCHAR(3000), " +
                "  @DebitGroupIDs    VARCHAR(3000), " +
                 " @ProductTypeIDs    VARCHAR(100), " +
                 " @GeneralSettings    VARCHAR(1000), " +
                 " @NegativeBalance    NUMERIC(18, 0), " +
                "  @RoundOffBlock    FLOAT, " +
                 " @RoundOffMode    NUMERIC(18, 0), " +
                "  @ItemClassIDS2    VARCHAR(500), " +
                "  @SecondaryCCIDS    VARCHAR(500), " +
                "  @PrimaryCCIDS    VARCHAR(500), " +
                "  @OrderVchTypeIDS    VARCHAR(1000), " +
                "  @NoteVchTypeIDS    VARCHAR(1000), " +
                "  @QuotationVchTypeIDS    VARCHAR(1000), " +
                "  @DEFMOPID    INT, " +
                 " @BLNLOCKMOP    INT, " +
                "  @DEFTAXMODEID    INT, " +
                "  @BLNLOCKTAXMODE    INT, " +
                "  @DEFAGENTID    INT, " +
                "  @BLNLOCKAGENT    INT, " +
                "  @DEFPRICELISTID    INT, " +
                "  @BLNLOCKPRICELIST    INT, " +
                "  @DEFSALESMANID    INT, " +
                "  @BLNLOCKSALESMAN    INT, " +
                 " @DEFPRINTID    INT, " +
                 " @BLNLOCKPRINT    INT, " +
                "  @ColwidthStr    VARCHAR(2000), " +
                "  @gridColor    VARCHAR(50), " +
                "  @DefaultGodownID    NUMERIC(18, 0), " +
                 " @ActCFasCostLedger    NUMERIC(18, 0), " +
                 " @ActCFasCostLedger4    NUMERIC(18, 0), " +
                "  @gridHeaderColor    VARCHAR(50), " +
                "  @BLNUseForClientSync    NUMERIC(18, 0), " +
                "  @rateInclusiveIndex    NUMERIC(18, 0), " +
                "  @BlnBillWiseDisc    NUMERIC(18, 0), " +
                "  @BlnItemWisePerDisc    NUMERIC(18, 0), " +
                "  @BlnItemWiseAmtDisc    NUMERIC(18, 0), " +
                "  @gridselectedRow    VARCHAR(50), " +
                "  @GridHeaderFont    VARCHAR(50), " +
                "  @GridBackColor    VARCHAR(50), " +
                "  @GridAlternatCellColor    VARCHAR(50), " +
                 " @GridCellColor    VARCHAR(50), " +
                "  @GridFontColor    VARCHAR(50), " +
                 " @Metatag    NVARCHAR(3000), " +
                 " @DefaultCriteria    NVARCHAR(50), " +
                "  @SearchSql    NVARCHAR(MAX), " +
                "  @SmartSearchBehavourMode    NUMERIC(18, 0), " +
                "  @criteriaconfig    VARCHAR(MAX), " +
                "  @intEnterKeyBehavourMode    NUMERIC(18, 0), " +
                "  @BlnBillDiscAmtEntry    NUMERIC(18, 0), " +
                "  @blnRateDiscount    NUMERIC(18, 0), " +
                 " @IntdefaultFocusColumnID    NUMERIC(18, 0), " +
                 " @BlnTouchScreen    NUMERIC(18, 0), " +
                 " @StrTouchSetting    VARCHAR(MAX), " +
                 " @StrCalculationFields    VARCHAR(MAX), " +
                "  @CRateCalMethod    INT, " +
                "  @MMRPSortOrder    NUMERIC(18, 0), " +
                "  @ItemDiscountFrom    NUMERIC(18, 0), " +
                 " @DEFPRINTID2    NUMERIC(18, 0), " +
                "  @BLNLOCKPRINT2    NUMERIC(18, 0), " +
                "  @BillDiscountFrom    NUMERIC(18, 0), " +
                "  @WindowBackColor    VARCHAR(50), " +
                "  @ContrastBackColor    VARCHAR(50), " +
                 " @BlnEnableCustomFormColor    NUMERIC(18, 0), " +
                "  @returnVchtypeID    NUMERIC(18, 0), " +
                "  @PrintCopies    NUMERIC(18, 0), " +
                 " @SystemName    VARCHAR(50), " +
                "  @UserID    NUMERIC(18, 0), " +
                "  @LastUpdateDate    DATETIME, " +
                 " @LastUpdateTime    DATETIME, " +
                "  @BlnMobileVoucher    NUMERIC(18, 0), " +
                "  @SearchSQLSettings    VARCHAR(MAX), " +
                "  @AdvancedSearchSQLEnabled    NUMERIC(18, 0), " +
                 " @TenantID   NUMERIC(18, 0), " +
                 " @VchJson   VARCHAR(MAX), " +
                 " @FeaturesJson  VARCHAR(MAX), " +
                 " @DEFTaxInclusiveID    INT, " +
                "  @BLNLOCKTaxInclusive    INT, " +
               "  @Action             INT = 0 " +
            " ) " +
            " AS " +
            " BEGIN " +
            " DECLARE @RetResult      INT " +
            " BEGIN TRY " +
            " BEGIN TRANSACTION; " +
                        " IF @Action = 0 " +
            " BEGIN " +
                 " INSERT INTO tblVchType(VchTypeID, VchType, ShortKey, EasyKey, SortOrder, ActiveStatus, ParentID, Description, numberingCode, Prefix, Sufix, ItemClassIDS, CreditGroupIDs, DebitGroupIDs, ProductTypeIDs, GeneralSettings, NegativeBalance, RoundOffBlock, RoundOffMode, ItemClassIDS2, SecondaryCCIDS, PrimaryCCIDS, OrderVchTypeIDS, NoteVchTypeIDS, QuotationVchTypeIDS, DEFMOPID, BLNLOCKMOP, DEFTAXMODEID, BLNLOCKTAXMODE, DEFAGENTID, BLNLOCKAGENT, DEFPRICELISTID, BLNLOCKPRICELIST, DEFSALESMANID, BLNLOCKSALESMAN, DEFPRINTID, BLNLOCKPRINT, ColwidthStr, gridColor, DefaultGodownID, ActCFasCostLedger, ActCFasCostLedger4, gridHeaderColor, BLNUseForClientSync, rateInclusiveIndex, BlnBillWiseDisc, BlnItemWisePerDisc, BlnItemWiseAmtDisc, gridselectedRow, GridHeaderFont, GridBackColor, GridAlternatCellColor, GridCellColor, GridFontColor, Metatag, DefaultCriteria, SearchSql, SmartSearchBehavourMode, criteriaconfig, intEnterKeyBehavourMode, BlnBillDiscAmtEntry, blnRateDiscount, IntdefaultFocusColumnID, BlnTouchScreen, StrTouchSetting, StrCalculationFields, CRateCalMethod, MMRPSortOrder, ItemDiscountFrom, DEFPRINTID2, BLNLOCKPRINT2, BillDiscountFrom, WindowBackColor, ContrastBackColor, BlnEnableCustomFormColor, returnVchtypeID, PrintCopies, SystemName, UserID, LastUpdateDate, LastUpdateTime, BlnMobileVoucher, SearchSQLSettings, AdvancedSearchSQLEnabled, TenantID, VchJson, FeaturesJson, DEFTAXINCLUSIVEID, BLNLOCKTAXINCLUSIVE) " +
                 " VALUES(@VchTypeID, @VchType, @ShortKey, @EasyKey, @SortOrder, @ActiveStatus, @ParentID, @Description, @numberingCode, @Prefix, @Sufix, @ItemClassIDS, @CreditGroupIDs, @DebitGroupIDs, @ProductTypeIDs, @GeneralSettings, @NegativeBalance, @RoundOffBlock, @RoundOffMode, @ItemClassIDS2, @SecondaryCCIDS, @PrimaryCCIDS, @OrderVchTypeIDS, @NoteVchTypeIDS, @QuotationVchTypeIDS, @DEFMOPID, @BLNLOCKMOP, @DEFTAXMODEID, @BLNLOCKTAXMODE, @DEFAGENTID, @BLNLOCKAGENT, @DEFPRICELISTID, @BLNLOCKPRICELIST, @DEFSALESMANID, @BLNLOCKSALESMAN, @DEFPRINTID, @BLNLOCKPRINT, @ColwidthStr, @gridColor, @DefaultGodownID, @ActCFasCostLedger, @ActCFasCostLedger4, @gridHeaderColor, @BLNUseForClientSync, @rateInclusiveIndex, @BlnBillWiseDisc, @BlnItemWisePerDisc, @BlnItemWiseAmtDisc, @gridselectedRow, @GridHeaderFont, @GridBackColor, @GridAlternatCellColor, @GridCellColor, @GridFontColor, @Metatag, @DefaultCriteria, @SearchSql, @SmartSearchBehavourMode, @criteriaconfig, @intEnterKeyBehavourMode, @BlnBillDiscAmtEntry, @blnRateDiscount, @IntdefaultFocusColumnID, @BlnTouchScreen, @StrTouchSetting, @StrCalculationFields, @CRateCalMethod, @MMRPSortOrder, @ItemDiscountFrom, @DEFPRINTID2, @BLNLOCKPRINT2, @BillDiscountFrom, @WindowBackColor, @ContrastBackColor, @BlnEnableCustomFormColor, @returnVchtypeID, @PrintCopies, @SystemName, @UserID, @LastUpdateDate, @LastUpdateTime, @BlnMobileVoucher, @SearchSQLSettings, @AdvancedSearchSQLEnabled, @TenantID, @VchJson, @FeaturesJson, @DEFTaxInclusiveID, @BLNLOCKTAXINCLUSIVE) " +
                 " SET @RetResult = 1; " +
                       "  END " +
                        " IF @Action = 1 " +
            " BEGIN " +
                 " UPDATE tblVchType SET VchType = @VchType,ShortKey = @ShortKey,EasyKey = @EasyKey,SortOrder = @SortOrder,ActiveStatus = @ActiveStatus,ParentID = @ParentID,Description = @Description,numberingCode = @numberingCode,Prefix = @Prefix,Sufix = @Sufix,ItemClassIDS = @ItemClassIDS,CreditGroupIDs = @CreditGroupIDs,DebitGroupIDs = @DebitGroupIDs,ProductTypeIDs = @ProductTypeIDs,GeneralSettings = @GeneralSettings,NegativeBalance = @NegativeBalance,RoundOffBlock = @RoundOffBlock,RoundOffMode = @RoundOffMode,ItemClassIDS2 = @ItemClassIDS2,SecondaryCCIDS = @SecondaryCCIDS,PrimaryCCIDS = @PrimaryCCIDS,OrderVchTypeIDS = @OrderVchTypeIDS,NoteVchTypeIDS = @NoteVchTypeIDS,QuotationVchTypeIDS = @QuotationVchTypeIDS,DEFMOPID = @DEFMOPID,BLNLOCKMOP = @BLNLOCKMOP,DEFTAXMODEID = @DEFTAXMODEID,BLNLOCKTAXMODE = @BLNLOCKTAXMODE,DEFAGENTID = @DEFAGENTID,BLNLOCKAGENT = @BLNLOCKAGENT,DEFPRICELISTID = @DEFPRICELISTID,BLNLOCKPRICELIST = @BLNLOCKPRICELIST,DEFSALESMANID = @DEFSALESMANID,BLNLOCKSALESMAN = @BLNLOCKSALESMAN,DEFPRINTID = @DEFPRINTID,BLNLOCKPRINT = @BLNLOCKPRINT,ColwidthStr = @ColwidthStr,gridColor = @gridColor,DefaultGodownID = @DefaultGodownID,ActCFasCostLedger = @ActCFasCostLedger,ActCFasCostLedger4 = @ActCFasCostLedger4,gridHeaderColor = @gridHeaderColor,BLNUseForClientSync = @BLNUseForClientSync,rateInclusiveIndex = @rateInclusiveIndex,BlnBillWiseDisc = @BlnBillWiseDisc,BlnItemWisePerDisc = @BlnItemWisePerDisc,BlnItemWiseAmtDisc = @BlnItemWiseAmtDisc,gridselectedRow = @gridselectedRow,GridHeaderFont = @GridHeaderFont,GridBackColor = @GridBackColor,GridAlternatCellColor = @GridAlternatCellColor,GridCellColor = @GridCellColor,GridFontColor = @GridFontColor,Metatag = @Metatag,DefaultCriteria = @DefaultCriteria,SearchSql = @SearchSql,SmartSearchBehavourMode = @SmartSearchBehavourMode,criteriaconfig = @criteriaconfig,intEnterKeyBehavourMode = @intEnterKeyBehavourMode,BlnBillDiscAmtEntry = @BlnBillDiscAmtEntry,blnRateDiscount = @blnRateDiscount,IntdefaultFocusColumnID = @IntdefaultFocusColumnID,BlnTouchScreen = @BlnTouchScreen,StrTouchSetting = @StrTouchSetting,StrCalculationFields = @StrCalculationFields,CRateCalMethod = @CRateCalMethod,MMRPSortOrder = @MMRPSortOrder,ItemDiscountFrom = @ItemDiscountFrom,DEFPRINTID2 = @DEFPRINTID2,BLNLOCKPRINT2 = @BLNLOCKPRINT2,BillDiscountFrom = @BillDiscountFrom,WindowBackColor = @WindowBackColor,ContrastBackColor = @ContrastBackColor,BlnEnableCustomFormColor = @BlnEnableCustomFormColor,returnVchtypeID = @returnVchtypeID,PrintCopies = @PrintCopies,SystemName = @SystemName,UserID = @UserID,LastUpdateDate = @LastUpdateDate,LastUpdateTime = @LastUpdateTime,BlnMobileVoucher = @BlnMobileVoucher,SearchSQLSettings = @SearchSQLSettings,AdvancedSearchSQLEnabled = @AdvancedSearchSQLEnabled,VchJson = @VchJson,FeaturesJson = @FeaturesJson,DEFTAXINCLUSIVEID = @DEFTaxInclusiveID,BLNLOCKTAXINCLUSIVE = @BLNLOCKTAXINCLUSIVE " +
                 " WHERE VchTypeID = @VchTypeID AND TenantID = @TenantID " +

                 " SET @RetResult = 1; " +
                      "   END " +
                        " IF @Action = 2 " +
            " BEGIN " +
                 " DELETE FROM tblVchType WHERE VchTypeID = @VchTypeID AND TenantID = @TenantID " +
                "  SET @RetResult = 0; " +
                        " END " +
                        " COMMIT TRANSACTION; " +
                       "  SELECT @RetResult as SqlSpResult " +
            " END TRY " +
            " BEGIN CATCH " +
            " ROLLBACK; " +
                        " SELECT " +
                        " - 1 as SqlSpResult, " +
            " ERROR_NUMBER() AS ErrorNumber, " +
            " ERROR_STATE() AS ErrorState, " +
            " ERROR_SEVERITY() AS ErrorSeverity, " +
            " ERROR_PROCEDURE() AS ErrorProcedure, " +
            " ERROR_LINE() AS ErrorLine, " +
            " ERROR_MESSAGE() AS ErrorMessage; " +
                        " END CATCH; " +
                        " END";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspItemMasterInsert') " +
                       "DROP PROCEDURE UspItemMasterInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspItemMasterInsert] " +
           "( " +
                   "@ItemID    NUMERIC  (18,0), " +
               "@ItemCode    VARCHAR  (100), " +
                   "@ItemName    VARCHAR  (100), " +
                   "@CategoryID    NUMERIC  (18,0), " +
                   "@Description    VARCHAR  (1000), " +
                   "@PRate    MONEY, " +
                   "@SrateCalcMode    INT, " +
                   "@CRateAvg    MONEY, " +
                   "@Srate1Per    MONEY, " +
                   "@SRate1    MONEY, " +
                   "@Srate2Per    MONEY, " +
                   "@SRate2    MONEY, " +
                   "@Srate3Per    MONEY, " +
                   "@SRate3    MONEY, " +
                   "@Srate4    MONEY, " +
                   "@Srate4Per    MONEY, " +
                   "@SRate5    MONEY, " +
                   "@Srate5Per    MONEY, " +
                   "@MRP    MONEY, " +
                   "@ROL    FLOAT, " +
                   "@Rack    VARCHAR  (100), " +
                   "@Manufacturer    VARCHAR  (100), " +
                   "@ActiveStatus    NUMERIC  (1,0), " +
                   "@IntLocal    INT, " +
                   "@ProductType    VARCHAR  (50), " +
                   "@ProductTypeID    FLOAT, " +
                   "@LedgerID    NUMERIC  (18,0), " +
                   "@UNITID    NUMERIC  (18,0), " +
                   "@Notes    VARCHAR  (1000), " +
                   "@agentCommPer    FLOAT, " +
                   "@BlnExpiryItem    INT, " +
                   "@Coolie    NUMERIC  (18,0), " +
                   "@FinishedGoodID    INT, " +
                   "@MinRate    FLOAT, " +
                   "@MaxRate    FLOAT, " +
                   "@PLUNo    NUMERIC(18,0), " +
                   "@HSNID    NUMERIC  (18,0), " +
                   "@iCatDiscPer    FLOAT, " +
                   "@IPGDiscPer    FLOAT, " +
                   "@ImanDiscPer    FLOAT, " +
                   "@ItemNameUniCode    NVARCHAR  (500), " +
                   "@Minqty    FLOAT, " +
                   "@MNFID    NUMERIC  (18,0), " +
                   "@PGID    NUMERIC  (18,0), " +
                   "@ItemCodeUniCode    NVARCHAR  (50), " +
                   "@UPC    VARCHAR  (50), " +
                   "@BatchMode    VARCHAR  (50), " +
                   "@blnExpiry    NUMERIC  (1,0), " +
                   "@Qty    FLOAT, " +
                   "@MaxQty    FLOAT, " +
                   "@IntNoOrWeight    NUMERIC  (18,0), " +
                   "@SystemName    VARCHAR  (50), " +
                   "@UserID    NUMERIC  (18,0), " +
                   "@LastUpdateDate    DATETIME, " +
                   "@LastUpdateTime    DATETIME, " +
                   "@TenantID    NUMERIC  (18,0), " +
                   "@blnCessOnTax    NUMERIC  (18,0), " +
                   "@CompCessQty    FLOAT, " +
                   "@CGSTTaxPer    FLOAT, " +
                   "@SGSTTaxPer    FLOAT, " +
                   "@IGSTTaxPer    FLOAT, " +
                   "@CessPer    FLOAT, " +
                   "@VAT    FLOAT, " +
                   "@CategoryIDs    VARCHAR  (1000), " +
                   "@ColorIDs    VARCHAR  (1000), " +
                   "@SizeIDs    VARCHAR  (1000), " +
                   "@BrandDisPer    FLOAT, " +
                   "@DGroupID    NUMERIC  (18,0), " +
                   "@DGroupDisPer   FLOAT, " +
                   "@Action             INT=0, " +
                   "@BatchCode		VARCHAR(50), " +
                   "@CostRateInc		FLOAT, " +
                   "@CostRateExcl		FLOAT, " +
                   "@PRateExcl			FLOAT, " +
                   "@PrateInc			FLOAT, " +
                   "@BrandID			NUMERIC  (18,0), " +
                   "@AltUnitID			NUMERIC  (18,0), " +
                   "@ConvFactor			NUMERIC  (18,5), " +
                   "@ShelfLife NUMERIC  (18,0), " +
                   "@SRateInclusive NUMERIC  (1,0), " +
                   "@PRateInclusive NUMERIC  (1,0), " +
                   "@Slabsys NUMERIC  (1,0), " +
                   "@DiscPer FLOAT, " +
                   "@DepartmentID NUMERIC  (18,0), " +
                   "@DefaultExpInDays NUMERIC  (18,0) " +
                   " ) " +
                   "AS " +
                   "BEGIN " +
                   "DECLARE @RetResult      INT " +
                   "DECLARE @BatchID		NUMERIC(18,0) " +
                   "DECLARE @StockID		NUMERIC(18,0) " +
                   "DECLARE @ExpDt			DATETIME = DATEADD(Year,8,getdate()) " +
                   "DECLARE @LastInvDt			DATETIME = Getdate() " +
                   "DECLARE @GetPLUNo		NUMERIC(18,0) " +
                   "DECLARE @TransType		CHAR(1) " +
                   "BEGIN TRY " +
                   "BEGIN TRANSACTION; " +
               "IF NOT EXISTS(SELECT * FROM tblOnetimeMaster WHERE OtmType = 'ITMRACK' AND LTRIM(RTRIM(OtmData)) = @Rack) " +
               "BEGIN " +
                   "INSERT INTO tblOnetimeMaster(OtmData,OtmValue,OtmDescription,OtmType,TenantID) " +
                   "VALUES(@Rack,0,'Rack Details','ITMRACK',@TenantID) " +
               "END " +
               "IF NOT EXISTS(SELECT * FROM tblOnetimeMaster WHERE OtmType = 'PRODCLASS' AND LTRIM(RTRIM(OtmData)) = @ProductType) " +
               "BEGIN " +
                   "INSERT INTO tblOnetimeMaster(OtmData,OtmValue,OtmDescription,OtmType,TenantID) " +
                   "VALUES(@ProductType,0,'Product Class Details','PRODCLASS',@TenantID) " +
               "END " +
               "SELECT @iCatDiscPer = ISNULL(CatDiscPer,0) FROM tblCategories WHERE TenantID = @TenantID AND CategoryID = @CategoryID " +
               "SELECT @ImanDiscPer = ISNULL(DiscPer,0) FROM tblManufacturer WHERE TenantID = @TenantID AND MnfID = @MNFID " +
               "SELECT @BrandDisPer = ISNULL(DiscPer,0) FROM tblBrand WHERE TenantID = @TenantID AND brandID = @BrandID " +
               "SELECT @DGroupDisPer = ISNULL(DiscPer,0) FROM tblDiscountGroup WHERE TenantID = @TenantID AND DiscountGroupID = @DGroupID " +
               "IF @Action = 0 " +
               "BEGIN " +
                   "SELECT @GetPLUNo = ISNULL(MAX(PLUNo) + 1,0) FROM tblItemMaster  " +
                   "IF @BatchMode = 3 " +
                   "BEGIN " +
                       "IF @PLUNo>0 " +
                       "BEGIN " +
                           "SET @PLUNo = @PLUNo " +
                       "END " +
                       "ELSE " +
                       "BEGIN " +
                           "SET @PLUNo = @GetPLUNo " +
                       "END " +
                   "END " +
                   "ELSE " +
                   "BEGIN " +
                       "SET @PLUNo = 0 " +
                   "END " +
                   "IF NOT EXISTS(SELECT * FROM tblStock WHERE TenantID = @TenantID) " +
                   "BEGIN " +
                       "SET @StockID = 1 " +
                       "SET @BatchID = 1 " +
                   "END " +
                   "ELSE " +
                   "BEGIN " +
                       "SELECT @StockID = ISNULL(MAX(StockID) + 1,0) FROM tblStock WHERE TenantID = @TenantID " +
                       "SELECT @BatchID = ISNULL(MAX(BatchID) + 1,0) FROM tblStock WHERE TenantID = @TenantID " +
                   "END " +
                       "INSERT INTO tblItemMaster(ItemID,ItemCode,ItemName,CategoryID,Description,PRate,SrateCalcMode,CRateAvg,Srate1Per,SRate1,Srate2Per,SRate2,Srate3Per,SRate3,Srate4,Srate4Per,SRate5,Srate5Per,MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,PGID,ItemCodeUniCode,UPC,BatchMode,blnExpiry,Qty,MaxQty,IntNoOrWeight,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID,blnCessOnTax,CompCessQty,CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,CessPer,VAT,CategoryIDs,ColorIDs,SizeIDs,BrandDisPer,DGroupID,DGroupDisPer,BrandID,AltUnitID,ConvFactor,ShelfLife,SRateInclusive,PRateInclusive,Slabsys,DiscPer,DepartmentID,DefaultExpInDays) " +
                       "VALUES(@ItemID,@ItemCode,@ItemName,@CategoryID,@Description,@PRate,@SrateCalcMode,@CRateAvg,@Srate1Per,@SRate1,@Srate2Per,@SRate2,@Srate3Per,@SRate3,@Srate4,@Srate4Per,@SRate5,@Srate5Per,@MRP,@ROL,@Rack,@Manufacturer,@ActiveStatus,@IntLocal,@ProductType,@ProductTypeID,@LedgerID,@UNITID,@Notes,@agentCommPer,@BlnExpiryItem,@Coolie,@FinishedGoodID,@MinRate,@MaxRate,@PLUNo,@HSNID,@iCatDiscPer,@IPGDiscPer,@ImanDiscPer,@ItemNameUniCode,@Minqty,@MNFID,@PGID,@ItemCodeUniCode,@UPC,@BatchMode,@blnExpiry,@Qty,@MaxQty,@IntNoOrWeight,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID,@blnCessOnTax,@CompCessQty,@CGSTTaxPer,@SGSTTaxPer,@IGSTTaxPer,@CessPer,@VAT,@CategoryIDs,@ColorIDs,@SizeIDs,@BrandDisPer,@DGroupID,@DGroupDisPer,@BrandID,@AltUnitID,@ConvFactor,@ShelfLife,@SRateInclusive,@PRateInclusive,@Slabsys,@DiscPer,@DepartmentID,@DefaultExpInDays) " +
                       "SET @RetResult = 1; " +
                       "SET @TransType = 'S'; " +
               "END " +
               "IF @Action = 1 " +
               "BEGIN " +
                   "SELECT @GetPLUNo = ISNULL(MAX(PLUNo) + 1,0) FROM tblItemMaster  " +
                   "IF @BatchMode = 3 " +
                   "BEGIN " +
                       "IF @PLUNo = 0 " +
                       "BEGIN " +
                           "SET @PLUNo = @GetPLUNo " +
                       "END " +
                   "END " +
                   "IF @ActiveStatus = 0 " +
                   "BEGIN " +
                       "UPDATE tblItemMaster SET ActiveStatus=@ActiveStatus WHERE ItemID=@ItemID AND TenantID=@TenantID " +
                   "END " +
                   "ELSE " +
                   "BEGIN " +
                       "UPDATE tblItemMaster SET ItemCode=@ItemCode,ItemName=@ItemName,CategoryID=@CategoryID,Description=@Description,PRate=@PRate,SrateCalcMode=@SrateCalcMode,CRateAvg=@CRateAvg,Srate1Per=@Srate1Per,SRate1=@SRate1,Srate2Per=@Srate2Per,SRate2=@SRate2,Srate3Per=@Srate3Per,SRate3=@SRate3,Srate4=@Srate4,Srate4Per=@Srate4Per,SRate5=@SRate5,Srate5Per=@Srate5Per,MRP=@MRP,ROL=@ROL,Rack=@Rack,Manufacturer=@Manufacturer,ActiveStatus=@ActiveStatus,IntLocal=@IntLocal,ProductType=@ProductType,ProductTypeID=@ProductTypeID,LedgerID=@LedgerID,UNITID=@UNITID,Notes=@Notes,agentCommPer=@agentCommPer,BlnExpiryItem=@BlnExpiryItem,Coolie=@Coolie,FinishedGoodID=@FinishedGoodID,MinRate=@MinRate,MaxRate=@MaxRate,PLUNo=@PLUNo,HSNID=@HSNID,iCatDiscPer=@iCatDiscPer,IPGDiscPer=@IPGDiscPer,ImanDiscPer=@ImanDiscPer,ItemNameUniCode=@ItemNameUniCode,Minqty=@Minqty,MNFID=@MNFID,PGID=@PGID,ItemCodeUniCode=@ItemCodeUniCode,UPC=@UPC,BatchMode=@BatchMode,blnExpiry=@blnExpiry,Qty=@Qty,MaxQty=@MaxQty,IntNoOrWeight=@IntNoOrWeight,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime,blnCessOnTax=@blnCessOnTax,CompCessQty=@CompCessQty,CGSTTaxPer=@CGSTTaxPer,SGSTTaxPer=@SGSTTaxPer,IGSTTaxPer=@IGSTTaxPer,CessPer=@CessPer,VAT=@VAT,CategoryIDs=@CategoryIDs,ColorIDs=@ColorIDs,SizeIDs=@SizeIDs,BrandDisPer=@BrandDisPer,DGroupID=@DGroupID,DGroupDisPer=@DGroupDisPer,BrandID = @BrandID,AltUnitID=@AltUnitID,ConvFactor=@ConvFactor,ShelfLife=@ShelfLife,SRateInclusive=@SRateInclusive,PRateInclusive=@PRateInclusive,Slabsys=@Slabsys,DiscPer=@DiscPer,DepartmentID=@DepartmentID,DefaultExpInDays=@DefaultExpInDays " +
                       "WHERE ItemID=@ItemID AND TenantID=@TenantID " +
                       "SELECT @StockID = StockID FROM tblStock WHERE BatchCode = @BatchCode AND TenantID = @TenantID AND ItemID = @ItemID " +
                       "set @StockID=ISNULL(@StockID,0) " +
                   "END " +
                   "SET @RetResult = 1; " +
                   "SET @TransType = 'E'; " +
               "END " +
               "IF @Action = 2 " +
               "BEGIN " +
                       "DELETE FROM tblStock WHERE ItemID = @ItemID AND TenantID=@TenantID   " +
                       "DELETE FROM tblItemMaster WHERE ItemID=@ItemID AND TenantID=@TenantID " +
                       "SET @RetResult = 0; " +
                       "SET @TransType = 'D'; " +
               "END " +
           "COMMIT TRANSACTION; " +
           "SELECT @RetResult as SqlSpResult,@ItemID as TransID,@TransType as TransactType " +
           "END TRY " +
           "BEGIN CATCH " +
           "ROLLBACK; " +
           "SELECT " +
           "-1 as SqlSpResult, " +
           "ERROR_NUMBER() AS ErrorNumber, " +
           "ERROR_STATE() AS ErrorState, " +
           "ERROR_SEVERITY() AS ErrorSeverity, " +
           "ERROR_PROCEDURE() AS ErrorProcedure, " +
           "ERROR_LINE() AS ErrorLine, " +
           "ERROR_MESSAGE() AS ErrorMessage; " +
           "END CATCH; " +
           "END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetItemMaster') " +
                       "DROP PROCEDURE UspGetItemMaster ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetItemMaster] ( @ItemID    NUMERIC   (18,0), @TenantID     NUMERIC   (18,0) )  " +
            "AS  BEGIN 	 " +
            "DECLARE @CatIDsNames	VARCHAR(1000) 	DECLARE @CatIDs	VARCHAR(1000)      IF @ItemID <> 0       BEGIN 	 " +
            "SELECT @CatIDs = CategoryIDs FROM tblItemMaster WHERE ItemID = @ItemID AND TenantID = @TenantID  " +
            "SELECT @CatIDsNames = COALESCE(@CatIDsNames + ',', '') + Category  		FROM tblCategories   " +
            "WHERE TenantID = @TenantID AND ','+ @CatIDs +',' LIKE '%,'+CONVERT(VARCHAR(50),CategoryID)+',%';    " +
            "SELECT I.ItemID,ItemCode,ItemName,I.CategoryID,Description,ISNULL(PRate,0) as PRate,ISNULL(SrateCalcMode,0) as SrateCalcMode,CRateAvg,Srate1Per,I.SRate1,Srate2Per, " +
            "I.SRate2,Srate3Per,I.SRate3,I.Srate4,Srate4Per,I.SRate5,Srate5Per,ISNULL(I.MRP,0) as MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID, " +
            "LedgerID,I.UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID, " +
            "PGID,ItemCodeUniCode,UPC,BatchMode,blnExpiry,Qty,MaxQty,IntNoOrWeight,I.SystemName,I.UserID,I.LastUpdateDate,I.LastUpdateTime,I.TenantID,blnCessOnTax,CompCessQty, " +
            "CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,CessPer,VAT,CategoryIDs,ColorIDs,SizeIDs,BrandDisPer,DGroupID,DGroupDisPer,@CatIDsNames as Categories, " +
            "U.UnitShortName as [Unit],ISNULL(BatchCode,0) as BatchCode,BrandID,ISNULL(AltUnitID,0) as AltUnitID,ISNULL(ConvFactor,0) as ConvFactor, " +
            "ISNULL(Shelflife,0) as Shelflife,ISNULL(SRateInclusive,0) as SRateInclusive,ISNULL(PRateInclusive,0) as PRateInclusive,ISNULL(Slabsys,0) as Slabsystem,batchMode, " +
            "ISNULL(DiscPer,0) AS DiscPer,S.BatchUnique, S.StockID,ISNULL(DepartmentID,0) as DepartmentID,ISNULL(CompCessQty,0) as CompCessQty,ISNULL(DefaultExpInDays,0) as DefaultExpInDays " +
            "FROM tblItemMaster I  " +
            "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID  " +
            "LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID 	 " +
            "LEFT JOIN tblStock S ON S.ItemID =I.ItemID     " +
            "WHERE I.ItemID = @ItemID AND I.TenantID = @TenantID 	 END      ELSE      BEGIN  " +
            "SELECT I.ItemID,ItemCode as [Item Code],ItemName as [Item],U.UnitShortName as [Unit],C.Category,Description,I.MRP,HSNID as [HSN Code], " +
            "(CASE WHEN ActiveStatus = 1 THEN 'Active' ELSE 'In Active' END) as Status, 		 ISNULL(BatchCode,0) as BatchCode 		 FROM tblItemMaster I  	 " +
            "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID 		 LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID 	 " +
            "LEFT JOIN tblStock S ON S.ItemID = I.ItemID          WHERE I.TenantID = @TenantID      END  END  ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'Uspgetstockreport') " +
                       "DROP PROCEDURE Uspgetstockreport ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE[dbo].[Uspgetstockreport] (@ToDate   DATETIME,  " +
                "                                             @TenantID NUMERIC(18, 0))  " +
                "   AS  " +
                "     BEGIN  " +
                "         DECLARE @CurrencyDecimal NUMERIC(18, 0) = (SELECT valuename FROM tblappsettings WHERE  keyname = 'CurrencyDecimals')  " +
                "         DECLARE @QtyDecimal NUMERIC(18, 0) = (SELECT valuename FROM   tblappsettings  WHERE  keyname = 'QtyDecimalFormat')  " +
                "   	  SELECT ItemName as [Item Name],SUM(Round(Isnull(qtyin, 0), @QtyDecimal))  AS[Qty In], SUM(Round(Isnull(qtyout, 0), @QtyDecimal)) AS[Qty Out],  " +
                "   	  (SUM(Round(Isnull(qtyin, 0), @QtyDecimal)) - SUM(Round(Isnull(qtyout, 0), @QtyDecimal))) as [QOH],S.ItemID  " +
                "   	  FROM tblstock S   " +
                "   	  INNER JOIN tblItemMaster I ON I.ItemID = S.ItemID  " +
                "   	  INNER JOIN tblStockHistory H ON H.ItemID = S.ItemID  " +
                "   	  WHERE H.TenantID = @TenantID AND CONVERT(DATETIME, H.VchDate, 106) <= @ToDate  " +
                "   	  GROUP BY S.ItemID,ItemName  " +
                "     END  ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "ALTER TABLE tblStock ADD PRate Numeric(18,5)  ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspStockInsert') " +
                      "DROP PROCEDURE UspStockInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspStockInsert] ( " +
                "    @StockID NUMERIC (18, 0), " +
                "    @TenantID NUMERIC (18, 0), " +
                "    @CCID NUMERIC (18, 0), " +
                "    @BatchCode VARCHAR(100), " +
                "    @BatchUnique VARCHAR (50), " +
                "    @BatchID NUMERIC (18, 0), " +
                "    @MRP NUMERIC(18, 5), " +
                "    @ExpiryDate DATE, " +
                "    @CostRateInc DECIMAL(18, 2), " +
                "    @CostRateExcl DECIMAL(18, 2), " +
                "    @PRateExcl DECIMAL(18, 2), " +
                "    @PrateInc DECIMAL(18, 2), " +
                "    @TaxPer DECIMAL(18, 2), " +
                "    @SRate1 DECIMAL(18, 2), " +
                "    @SRate2 DECIMAL(18, 2), " +
                "    @SRate3 DECIMAL(18, 2), " +
                "    @SRate4 DECIMAL(18, 2), " +
                "    @SRate5 DECIMAL(18, 2), " +
                "    @QOH DECIMAL, " +
                "    @LastInvDate DATE, " +
                "    @LastInvNo VARCHAR (50), " +
                "    @LastSupplierID NUMERIC (18, 0), " +
                "    @Action INT = 0, " +
                "    @ItemID NUMERIC (18, 0), " +
                "    @BatchMode VARCHAR(100), " +
                "	@PRate NUMERIC(18, 5)" +
                "  ) AS BEGIN DECLARE @RetResult INT DECLARE @TransType CHAR(1) DECLARE @blnExpiry NUMERIC(18, 0) BEGIN TRY BEGIN TRANSACTION;" +
                "IF @BatchMode = 0 BEGIN " +
                "SELECT " +
                "  @BatchCode = ItemCode, " +
                "  @blnExpiry = ISNULL(blnExpiry, 0) " +
                "FROM " +
                "  tblItemMaster " +
                "WHERE " +
                "  ItemID = @ItemID END IF @Action = 0 BEGIN IF @BatchMode = 0 BEGIN " +
                "  /*None*/" +
                "  INSERT INTO tblStock( " +
                "    StockID, TenantID, CCID, BatchCode, " +
                "    BatchUnique, BatchID, MRP, ExpiryDate, " +
                "    CostRateInc, CostRateExcl, PRateExcl, " +
                "    PrateInc, TaxPer, SRate1, SRate2, " +
                "    SRate3, SRate4, SRate5, QOH, LastInvDate, " +
                "    LastInvNo, LastSupplierID, ItemID,PRate " +
                "  ) " +
                "VALUES " +
                "  (" +
                "    @StockID, " +
                "    @TenantID, " +
                "    @CCID, " +
                "    @BatchCode, " +
                "    @BatchUnique, " +
                "    @BatchID, " +
                "    @MRP, " +
                "    @ExpiryDate, " +
                "    @CostRateInc, " +
                "    @CostRateExcl, " +
                "    @PRateExcl, " +
                "    @PrateInc, " +
                "    @TaxPer, " +
                "    @SRate1, " +
                "    @SRate2, " +
                "    @SRate3, " +
                "    @SRate4, " +
                "    @SRate5, " +
                "    ABS(@QOH), " +
                "    @LastInvDate, " +
                "    @LastInvNo, " +
                "    @LastSupplierID, " +
                "    @ItemID, " +
                "	@PRate " +
                "  ) " +
                "SET " +
                "  @RetResult = 1; " +
                "SET " +
                "  @TransType = 'S'; " +
                "END ELSE IF @BatchMode = 1 BEGIN INSERT INTO tblStock( " +
                "  StockID, TenantID, CCID, BatchCode, " +
                "  BatchUnique, BatchID, MRP, ExpiryDate, " +
                "  CostRateInc, CostRateExcl, PRateExcl, " +
                "  PrateInc, TaxPer, SRate1, SRate2, " +
                "  SRate3, SRate4, SRate5, QOH, LastInvDate, " +
                "  LastInvNo, LastSupplierID, ItemID,PRate " +
                ") " +
                "VALUES " +
                "  (" +
                "    @StockID, " +
                "    @TenantID, " +
                "    @CCID, " +
                "    @BatchCode, " +
                "    @BatchUnique, " +
                "    @BatchID, " +
                "    @MRP, " +
                "    @ExpiryDate, " +
                "    @CostRateInc, " +
                "    @CostRateExcl, " +
                "    @PRateExcl, " +
                "    @PrateInc, " +
                "    @TaxPer, " +
                "    @SRate1, " +
                "    @SRate2, " +
                "    @SRate3, " +
                "    @SRate4, " +
                "    @SRate5, " +
                "    ABS(@QOH), " +
                "    @LastInvDate, " +
                "    @LastInvNo, " +
                "    @LastSupplierID, " +
                "    @ItemID, " +
                "	@PRate " +
                "  ) " +
                "SET " +
                "  @RetResult = 1; " +
                "SET " +
                "  @TransType = 'S'; " +
                "END ELSE IF @BatchMode = 2 " +
                "AND @BatchCode <> '' BEGIN " +
                "/*Auto*/" +
                "INSERT INTO tblStock( " +
                "  StockID, TenantID, CCID, BatchCode, " +
                "  BatchUnique, BatchID, MRP, ExpiryDate, " +
                "  CostRateInc, CostRateExcl, PRateExcl, " +
                "  PrateInc, TaxPer, SRate1, SRate2, " +
                "  SRate3, SRate4, SRate5, QOH, LastInvDate, " +
                "  LastInvNo, LastSupplierID, ItemID " +
                ") " +
                "VALUES " +
                "  (" +
                "    @StockID, " +
                "    @TenantID, " +
                "    @CCID, " +
                "    @BatchCode, " +
                "    @BatchUnique, " +
                "    @BatchID, " +
                "    @MRP, " +
                "    @ExpiryDate, " +
                "    @CostRateInc, " +
                "    @CostRateExcl, " +
                "    @PRateExcl, " +
                "    @PrateInc, " +
                "    @TaxPer, " +
                "    @SRate1, " +
                "    @SRate2, " +
                "    @SRate3, " +
                "    @SRate4, " +
                "    @SRate5, " +
                "    ABS(@QOH), " +
                "    @LastInvDate, " +
                "    @LastInvNo, " +
                "    @LastSupplierID, " +
                "    @ItemID " +
                "  ) " +
                "SET " +
                "  @RetResult = 1; " +
                "SET " +
                "  @TransType = 'S'; " +
                "END ELSE IF @BatchMode = 3 BEGIN INSERT INTO tblStock( " +
                "  StockID, TenantID, CCID, BatchCode, " +
                "  BatchUnique, BatchID, MRP, ExpiryDate, " +
                "  CostRateInc, CostRateExcl, PRateExcl, " +
                "  PrateInc, TaxPer, SRate1, SRate2, " +
                "  SRate3, SRate4, SRate5, QOH, LastInvDate, " +
                "  LastInvNo, LastSupplierID, ItemID,PRate " +
                ") " +
                "VALUES " +
                "  (" +
                "    @StockID, " +
                "    @TenantID, " +
                "    @CCID, " +
                "    @BatchCode, " +
                "    @BatchUnique, " +
                "    @BatchID, " +
                "    @MRP, " +
                "    @ExpiryDate, " +
                "    @CostRateInc, " +
                "    @CostRateExcl, " +
                "    @PRateExcl, " +
                "    @PrateInc, " +
                "    @TaxPer, " +
                "    @SRate1, " +
                "    @SRate2, " +
                "    @SRate3, " +
                "    @SRate4, " +
                "    @SRate5, " +
                "    ABS(@QOH), " +
                "    @LastInvDate, " +
                "    @LastInvNo, " +
                "    @LastSupplierID, " +
                "    @ItemID, " +
                "	@PRate " +
                "  ) " +
                "SET " +
                "  @RetResult = 1; " +
                "SET " +
                "  @TransType = 'S'; " +
                "END END IF @Action = 1 BEGIN " +
                "UPDATE " +
                "  tblStock " +
                "SET " +
                "  BatchID = @BatchID, " +
                "  MRP = @MRP, " +
                "  ExpiryDate = @ExpiryDate, " +
                "  CostRateInc = @CostRateInc, " +
                "  CostRateExcl = @CostRateExcl, " +
                "  PRateExcl = @PRateExcl, " +
                "  PrateInc = @PrateInc, " +
                "  TaxPer = @TaxPer, " +
                "  SRate1 = @SRate1, " +
                "  SRate2 = @SRate2, " +
                "  SRate3 = @SRate3, " +
                "  SRate4 = @SRate4, " +
                "  SRate5 = @SRate5, " +
                "  QOH = QOH + @QOH, " +
                "  LastInvDate = @LastInvDate, " +
                "  LastInvNo = @LastInvNo, " +
                "  LastSupplierID = @LastSupplierID, " +
                "  PRate = @PRate " +
                " WHERE " +
                "  ItemID = @ItemID " +
                "  AND CCID = @CCID " +
                "  AND BatchCode = @BatchCode " +
                "  AND BatchUnique = @BatchUnique " +
                "  AND TenantID = @TenantID " +
                "SET " +
                "  @RetResult = 1; " +
                "SET " +
                "  @TransType = 'E'; " +
                "END IF @Action = 2 BEGIN " +
                "UPDATE " +
                "  tblStock " +
                "SET " +
                "  CCID = @CCID, " +
                "  BatchCode = @BatchCode, " +
                "  BatchUnique = @BatchUnique, " +
                "  BatchID = @BatchID, " +
                "  MRP = @MRP, " +
                "  ExpiryDate = @ExpiryDate, " +
                "  CostRateInc = @CostRateInc, " +
                "  CostRateExcl = @CostRateExcl, " +
                "  PRateExcl = @PRateExcl, " +
                "  PrateInc = @PrateInc, " +
                "  TaxPer = @TaxPer, " +
                "  SRate1 = @SRate1, " +
                "  SRate2 = @SRate2, " +
                "  SRate3 = @SRate3, " +
                "  SRate4 = @SRate4, " +
                "  SRate5 = @SRate5, " +
                "  QOH = QOH + @QOH, " +
                "  LastInvDate = @LastInvDate, " +
                "  LastInvNo = @LastInvNo, " +
                "  LastSupplierID = @LastSupplierID , " +
                "  PRate = @PRate " +
                "WHERE " +
                "  ItemID = @ItemID " +
                "  AND CCID = @CCID " +
                "  AND BatchCode = @BatchCode " +
                "  AND BatchUnique = @BatchUnique " +
                "  AND TenantID = @TenantID " +
                "SET " +
                "  @RetResult = 0; " +
                "SET " +
                "  @TransType = 'D'; " +
                "END COMMIT TRANSACTION; " +
                "SELECT " +
                "  @RetResult as SqlSpResult, " +
                "  @StockID as TransID, " +
                "  @TransType as TransactType END TRY BEGIN CATCH ROLLBACK; " +
                "SELECT " +
                "  -1 as SqlSpResult, " +
                "  ERROR_NUMBER() AS ErrorNumber, " +
                "  ERROR_STATE() AS ErrorState, " +
                "  ERROR_SEVERITY() AS ErrorSeverity, " +
                "  ERROR_PROCEDURE() AS ErrorProcedure, " +
                "  ERROR_LINE() AS ErrorLine, " +
                "  ERROR_MESSAGE() AS ErrorMessage; " +
                "END CATCH; " +
                "END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdateFromItem') " +
                   "DROP PROCEDURE UspTransStockUpdateFromItem ";
            Comm.fnExecuteNonQuery(sQuery, false);
            sQuery = "CREATE PROCEDURE [dbo].[UspTransStockUpdateFromItem] ( " +
         "  @ItemID NUMERIC(18, 0), " +
         "  @BatchCode VARCHAR(50), " +
         "  @BatchUniq VARCHAR(50), " +
         "  @Qty NUMERIC(18, 5), " +
         "  @MRP NUMERIC(18, 5), " +
         "  @CostRateInc NUMERIC(18, 5), " +
         "  @CostRateExcl NUMERIC(18, 5), " +
         "  @PRateExcl NUMERIC(18, 5), " +
         "  @PrateInc NUMERIC(18, 5), " +
         "  @TaxPer NUMERIC(18, 5), " +
         "  @SRate1 NUMERIC(18, 5), " +
         "  @SRate2 NUMERIC(18, 5), " +
         "  @SRate3 NUMERIC(18, 5), " +
         "  @SRate4 NUMERIC(18, 5), " +
         "  @SRate5 NUMERIC(18, 5), " +
         "  @BatchMode INT, " +
         "  @VchType VARCHAR(100), " +
         "  @VchDate DATETIME, " +
         "  @ExpDt DATETIME, " +
         "  @Action VARCHAR(20), " +
         "  @RefID NUMERIC(18, 0), " +
         "  @VchTypeID NUMERIC(18, 0), " +
         "  @CCID NUMERIC(18, 0), " +
         "  @TenantID NUMERIC(18, 0)," +
         "  @PRate NUMERIC(18, 5)" +
         ") AS BEGIN DECLARE @BatchID NUMERIC(18, 0) DECLARE @StockID NUMERIC(18, 0) DECLARE @LastInvDt DATETIME = Getdate() DECLARE @STOCKHISID NUMERIC(18, 0) DECLARE @PRFXBATCH VARCHAR(10) DECLARE @Stock NUMERIC(18, 5) DECLARE @BarCode VARCHAR(50) DECLARE @BarUniq VARCHAR(100) DECLARE @CalcQOH NUMERIC(18, 5) DECLARE @BLNADVANCED INT DECLARE @blnExpiry BIT " +
         "SET " +
         "  @BarCode = @BatchCode " +
         "SELECT " +
         "  @StockID = ISNULL( " +
         "    MAX(StockID) + 1, " +
         "    0 " +
         "  ) " +
         "FROM " +
         "  tblStock " +
         "WHERE " +
         "  TenantID = @TenantID " +
         "SELECT " +
         "  @BatchID = ISNULL( " +
         "    MAX(BatchID) + 1, " +
         "    0 " +
         "  ) " +
         "FROM " +
         "  tblStock " +
         "WHERE " +
         "  TenantID = @TenantID " +
         "SELECT " +
         "  @STOCKHISID = ISNULL(" +
         "    MAX(STOCKHISID) + 1, " +
         "    0 " +
         "  ) " +
         "FROM " +
         "  tblStockHistory " +
         "WHERE " +
         "  TenantID = @TenantID " +
         "SELECT " +
         "  @BLNADVANCED = ISNULl(ValueName, 0) " +
         "FROM " +
         "  [tblAppSettings] " +
         "WHERE " +
         "  UPPER(" +
         "    LTRIM(" +
         "      RTRIM(KeyName)" +
         "    )" +
         "  ) = 'BLNADVANCED' " +
         "SELECT " +
         "  @Stock = ISNULL(QOH, 0) " +
         "FROM " +
         "  tblStock " +
         "WHERE " +
         "  ItemID = @ItemID " +
         "  AND BatchCode = @BatchCode " +
         "  AND TenantID = @TenantID " +
         "SELECT " +
         "  @blnExpiry = ISNULL(blnExpiry, 0) " +
         "FROM " +
         "  tblItemMaster " +
         "WHERE " +
         "  ItemID = @ItemID IF @StockID = 0 BEGIN " +
         "SET " +
         "  @StockID = 1 END IF @STOCKHISID = 0 BEGIN " +
         "SET " +
         "  @STOCKHISID = 1 END IF @BatchID = 0 BEGIN " +
         "SET " +
         "  @BatchID = 1 END IF @Action = 'STOCKADD' BEGIN IF @BatchCode = '<Auto Barcode>' BEGIN Declare @Prefix VARCHAR(50) Declare @BatchPrefix VARCHAR(50) IF @BLNADVANCED = 1 BEGIN " +
         "Select " +
         "  @BatchPrefix = ValueName " +
         "from " +
         "  tblAppSettings " +
         "where " +
         "  KeyName = 'STRBATCODEPREFIXSUFFIX' " +
         "set " +
         "  @BatchPrefix = (" +
         "    SELECT " +
         "      PARSENAME(" +
         "        REPLACE(@BatchPrefix, 'ƒ', ''), " +
         "        1" +
         "      )" +
         "  ) IF(@BatchPrefix = '<YEARMONTH>') BEGIN " +
         "SELECT " +
         "  @Prefix =(" +
         "    Select " +
         "      [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix)" +
         "  ) " +
         "SET " +
         "  @BatchCode = @Prefix + CONVERT(VARCHAR, @BatchID) END ELSE BEGIN " +
         "Select " +
         "  @BatchPrefix = ValueName " +
         "from " +
         "  tblAppSettings " +
         "where " +
         "  KeyName = 'STRBATCODEPREFIXSUFFIX' " +
         "set " +
         "  @BatchPrefix = (" +
         "    SELECT " +
         "      PARSENAME(" +
         "        REPLACE(@BatchPrefix, 'ƒ', ''), " +
         "        1" +
         "      )" +
         "  ) " +
         "SELECT " +
         "  @BatchCode = ISNULL(" +
         "    MAX(BatchID), " +
         "    0" +
         "  ) + 1 " +
         "FROM " +
         "  tblStock " +
         "WHERE " +
         "  TenantID = @TenantID " +
         "SET " +
         "  @BatchCode = @BatchPrefix + CONVERT(VARCHAR, @BatchID) END END ELSE BEGIN " +
         "SELECT " +
         "  @BatchCode = ISNULL(" +
         "    MAX(BatchID), " +
         "    0" +
         "  ) + 1 " +
         "FROM " +
         "  tblStock " +
         "WHERE " +
         "  TenantID = @TenantID END " +
         "SET " +
         "  @BatchUniq = @BatchCode + '@' + CONVERT(" +
         "    VARCHAR(22), " +
         "    CONVERT(" +
         "      NUMERIC(18, 2), " +
         "      @MRP" +
         "    )" +
         "  ) + '@' + REPLACE(" +
         "    CONVERT(" +
         "      VARCHAR(10), " +
         "      FORMAT(@ExpDt, 'dd-MM-yy')" +
         "    ), " +
         "    '-', " +
         "    ''" +
         "  ) " +
         "SET " +
         "  @BarUniq = @BarCode + '@' + CONVERT(" +
         "    VARCHAR(22), " +
         "    CONVERT(" +
         "      NUMERIC(18, 2), " +
         "      @MRP" +
         "    )" +
         "  ) + '@' + REPLACE(" +
         "    CONVERT(" +
         "      VARCHAR(10), " +
         "      FORMAT(@ExpDt, 'dd-MM-yy')" +
         "    ), " +
         "    '-', " +
         "    ''" +
         "  ) IF EXISTS(" +
         "    SELECT " +
         "      * " +
         "    FROM " +
         "      tblStock " +
         "    WHERE " +
         "      ItemID = @ItemID " +
         "      AND BatchCode = @BarCode " +
         "      AND TenantID = @TenantID " +
         "      AND BatchUnique = @BatchUniq " +
         "      AND CCID = @CCID" +
         "  ) BEGIN IF @VchTypeID <> 0 BEGIN EXEC UspStockInsert @StockID, " +
         "  @TenantID, " +
         "  @CCID, " +
         "  @BarCode, " +
         "  @BatchUniq, " +
         "  @BatchID, " +
         "  @MRP, " +
         "  @ExpDt, " +
         "  @CostRateInc, " +
         "  @CostRateExcl, " +
         "  @PRateExcl, " +
         "  @PrateInc, " +
         "  @TaxPer, " +
         "  @SRate1, " +
         "  @SRate2, " +
         "  @SRate3, " +
         "  @SRate4, " +
         "  @SRate5, " +
         "  @Qty, " +
         "  @LastInvDt, " +
         "  '', " +
         "  NULL, " +
         "  1, " +
         "  @ItemID, " +
         "  @BatchMode," +
         "  @PRate INSERT INTO tblStockHistory(" +
         "    VchType, VchDate, RefId, ItemID, QtyIn, " +
         "    QtyOut, BatchCode, BatchUnique, Expiry, " +
         "    MRP, CostRateInc, CostRateExcl, PRateExcl, " +
         "    PrateInc, TaxPer, SRate1, SRate2, " +
         "    SRate3, SRate4, SRate5, VchTypeID, " +
         "    CCID, STOCKHISID, TenantID, StockID" +
         "  ) " +
         "VALUES " +
         "  (" +
         "    @VchType, @VchDate, @RefID, @ItemID, " +
         "    @Qty, 0, @BarCode, @BatchUniq, @ExpDt, " +
         "    @MRP, @CostRateInc, @CostRateExcl, " +
         "    @PRateExcl, @PrateInc, @TaxPer, @SRate1, " +
         "    @SRate2, @SRate3, @SRate4, @SRate5, " +
         "    @VchTypeID, @CCID, @STOCKHISID, @TenantID, " +
         "    @StockID" +
         "  ) END END ELSE BEGIN IF EXISTS(" +
         "    SELECT " +
         "      * " +
         "    FROM " +
         "      tblStock " +
         "    WHERE " +
         "      ItemID = @ItemID " +
         "      AND BatchCode = @BarCode " +
         "      AND TenantID = @TenantID " +
         "      AND CCID = @CCID" +
         "  ) BEGIN IF @VchTypeID <> 0 BEGIN EXEC UspStockInsert @StockID, " +
         "  @TenantID, " +
         "  @CCID, " +
         "  @BarCode, " +
         "  @BarUniq, " +
         "  @BatchID, " +
         "  @MRP, " +
         "  @ExpDt, " +
         "  @CostRateInc, " +
         "  @CostRateExcl, " +
         "  @PRateExcl, " +
         "  @PrateInc, " +
         "  @TaxPer, " +
         "  @SRate1, " +
         "  @SRate2, " +
         "  @SRate3, " +
         "  @SRate4, " +
         "  @SRate5, " +
         "  @Qty, " +
         "  @LastInvDt, " +
         "  '', " +
         "  NULL, " +
         "  0, " +
         "  @ItemID, " +
         "  @BatchMode," +
         "  @PRate INSERT INTO tblStockHistory(" +
         "    VchType, VchDate, RefId, ItemID, QtyIn, " +
         "    QtyOut, BatchCode, BatchUnique, Expiry, " +
         "    MRP, CostRateInc, CostRateExcl, PRateExcl, " +
         "    PrateInc, TaxPer, SRate1, SRate2, " +
         "    SRate3, SRate4, SRate5, VchTypeID, " +
         "    CCID, STOCKHISID, TenantID, StockID" +
         "  ) " +
         "VALUES " +
         "  (" +
         "    @VchType, @VchDate, @RefID, @ItemID, " +
         "    @Qty, 0, @BarCode, @BarUniq, @ExpDt, " +
         "    @MRP, @CostRateInc, @CostRateExcl, " +
         "    @PRateExcl, @PrateInc, @TaxPer, @SRate1, " +
         "    @SRate2, @SRate3, @SRate4, @SRate5, " +
         "    @VchTypeID, @CCID, @STOCKHISID, @TenantID, " +
         "    @StockID" +
         "  ) END END ELSE BEGIN IF @VchTypeID <> 0 BEGIN EXEC UspStockInsert @StockID, " +
         "  @TenantID, " +
         "  @CCID, " +
         "  @BatchCode, " +
         "  @BatchUniq, " +
         "  @BatchID, " +
         "  @MRP, " +
         "  @ExpDt, " +
         "  @CostRateInc, " +
         "  @CostRateExcl, " +
         "  @PRateExcl, " +
         "  @PrateInc, " +
         "  @TaxPer, " +
         "  @SRate1, " +
         "  @SRate2, " +
         "  @SRate3, " +
         "  @SRate4, " +
         "  @SRate5, " +
         "  @Qty, " +
         "  @LastInvDt, " +
         "  '', " +
         "  NULL, " +
         "  0, " +
         "  @ItemID, " +
         "  @BatchMode," +
         "  @PRate INSERT INTO tblStockHistory(" +
         "    VchType, VchDate, RefId, ItemID, QtyIn, " +
         "    QtyOut, BatchCode, BatchUnique, Expiry, " +
         "    MRP, CostRateInc, CostRateExcl, PRateExcl, " +
         "    PrateInc, TaxPer, SRate1, SRate2, " +
         "    SRate3, SRate4, SRate5, VchTypeID, " +
         "    CCID, STOCKHISID, TenantID, StockID" +
         "  ) " +
         "VALUES " +
         "  (" +
         "    @VchType, @VchDate, @RefID, @ItemID, " +
         "    @Qty, 0, @BatchCode, @BatchUniq, @ExpDt, " +
         "    @MRP, @CostRateInc, @CostRateExcl, " +
         "    @PRateExcl, @PrateInc, @TaxPer, @SRate1, " +
         "    @SRate2, @SRate3, @SRate4, @SRate5, " +
         "    @VchTypeID, @CCID, @STOCKHISID, @TenantID, " +
         "    @StockID" +
         "  ) END END END END ELSE BEGIN IF @BatchMode = 0 BEGIN " +
         "SELECT " +
         "  @BatchCode = ItemCode " +
         "FROM " +
         "  tblItemMaster " +
         "WHERE " +
         "  ItemID = @ItemID " +
         "SET " +
         "  @BatchUniq = @BatchCode " +
         "SET " +
         "  @BarCode = @BatchCode END ELSE BEGIN " +
         "SET " +
         "  @BatchUniq = @BatchCode + '@' + CONVERT(" +
         "    VARCHAR(22), " +
         "    CONVERT(" +
         "      NUMERIC(18, 2), " +
         "      @MRP" +
         "    )" +
         "  ) IF CHARINDEX('@', @BatchUniq) = 0 BEGIN IF @blnExpiry = 1 BEGIN " +
         "SET " +
         "  @BatchUniq = @BatchCode + '@' + CONVERT(" +
         "    VARCHAR(22), " +
         "    CONVERT(" +
         "      NUMERIC(18, 2), " +
         "      @MRP" +
         "    )" +
         "  ) + '@' + REPLACE(" +
         "    CONVERT(" +
         "      VARCHAR(10), " +
         "      FORMAT(@ExpDt, 'dd-MM-yy')" +
         "    ), " +
         "    '-', " +
         "    ''" +
         "  ) END ELSE BEGIN " +
         "SET " +
         "  @BatchUniq = @BatchCode + '@' + CONVERT(" +
         "    VARCHAR(22), " +
         "    CONVERT(" +
         "      NUMERIC(18, 2), " +
         "      @MRP" +
         "    )" +
         "  ) END END END IF EXISTS(" +
         "    SELECT " +
         "      * " +
         "    FROM " +
         "      tblStock " +
         "    WHERE " +
         "      ItemID = @ItemID " +
         "      AND BatchCode = @BarCode " +
         "      AND TenantID = @TenantID " +
         "      AND BatchUnique = @BatchUniq " +
         "      AND CCID = @CCID" +
         "  ) BEGIN EXEC UspStockInsert @StockID, " +
         "  @TenantID, " +
         "  @CCID, " +
         "  @BatchCode, " +
         "  @BatchUniq, " +
         "  @BatchID, " +
         "  @MRP, " +
         "  @ExpDt, " +
         "  @CostRateInc, " +
         "  @CostRateExcl, " +
         "  @PRateExcl, " +
         "  @PrateInc, " +
         "  @TaxPer, " +
         "  @SRate1, " +
         "  @SRate2, " +
         "  @SRate3, " +
         "  @SRate4, " +
         "  @SRate5, " +
         "  @Qty, " +
         "  @LastInvDt, " +
         "  '', " +
         "  NULL, " +
         "  1, " +
         "  @ItemID, " +
         "  @BatchMode," +
         "  @PRate IF @VchTypeID <> 0 BEGIN INSERT INTO tblStockHistory(" +
         "    VchType, VchDate, RefId, ItemID, QtyIn, " +
         "    QtyOut, BatchCode, BatchUnique, Expiry, " +
         "    MRP, CostRateInc, CostRateExcl, PRateExcl, " +
         "    PrateInc, TaxPer, SRate1, SRate2, " +
         "    SRate3, SRate4, SRate5, VchTypeID, " +
         "    CCID, STOCKHISID, TenantID, StockID" +
         "  ) " +
         "VALUES " +
         "  (" +
         "    @VchType, @VchDate, @RefID, @ItemID, " +
         "    @Qty, 0, @BatchCode, @BatchUniq, @ExpDt, " +
         "    @MRP, @CostRateInc, @CostRateExcl, " +
         "    @PRateExcl, @PrateInc, @TaxPer, @SRate1, " +
         "    @SRate2, @SRate3, @SRate4, @SRate5, " +
         "    @VchTypeID, @CCID, @STOCKHISID, @TenantID, " +
         "    @StockID" +
         "  ) END END ELSE BEGIN EXEC UspStockInsert @StockID, " +
         "  @TenantID, " +
         "  @CCID, " +
         "  @BatchCode, " +
         "  @BatchUniq, " +
         "  @BatchID, " +
         "  @MRP, " +
         "  @ExpDt, " +
         "  @CostRateInc, " +
         "  @CostRateExcl, " +
         "  @PRateExcl, " +
         "  @PrateInc, " +
         "  @TaxPer, " +
         "  @SRate1, " +
         "  @SRate2, " +
         "  @SRate3, " +
         "  @SRate4, " +
         "  @SRate5, " +
         "  @Qty, " +
         "  @LastInvDt, " +
         "  '', " +
         "  NULL, " +
         "  0, " +
         "  @ItemID, " +
         "  @BatchMode," +
         "  @PRate IF @VchTypeID <> 0 BEGIN INSERT INTO tblStockHistory(" +
         "    VchType, VchDate, RefId, ItemID, QtyIn, " +
         "    QtyOut, BatchCode, BatchUnique, Expiry, " +
         "    MRP, CostRateInc, CostRateExcl, PRateExcl, " +
         "    PrateInc, TaxPer, SRate1, SRate2, " +
         "    SRate3, SRate4, SRate5, VchTypeID, " +
         "    CCID, STOCKHISID, TenantID, StockID" +
         "  ) " +
         "VALUES " +
         "  (" +
         "    @VchType, @VchDate, @RefID, @ItemID, " +
         "    @Qty, 0, @BatchCode, @BatchUniq, @ExpDt, " +
         "    @MRP, @CostRateInc, @CostRateExcl, " +
         "    @PRateExcl, @PrateInc, @TaxPer, @SRate1, " +
         "    @SRate2, @SRate3, @SRate4, @SRate5, " +
         "    @VchTypeID, @CCID, @STOCKHISID, @TenantID, " +
         "    @StockID" +
         "  ) END END END " +
         "SET " +
         "  @BatchCode = @BarCode END IF @Action = 'STOCKLESS' BEGIN " +
         "SET " +
         "  @Qty = @Qty * -1;" +
         "END IF @Action = 'STOCKDEL' BEGIN " +
         "SELECT " +
         "  @CalcQOH = QOH " +
         "FROM " +
         "  tblStock " +
         "WHERE " +
         "  ItemID = @ItemID " +
         "  AND CCID = @CCID " +
         "  AND BatchCode = @BatchCode " +
         "  AND BatchUnique = @BatchUniq " +
         "  AND TenantID = @TenantID " +
         "UPDATE " +
         "  tblStock " +
         "SET " +
         "  QOH = QOH - @CalcQOH " +
         "WHERE " +
         "  ItemID = @ItemID " +
         "  AND CCID = @CCID " +
         "  AND BatchCode = @BatchCode " +
         "  AND BatchUnique = @BatchUniq " +
         "  AND TenantID = @TenantID " +
         "DELETE FROM " +
         "  tblStockHistory " +
         "WHERE " +
         "  RefId = @RefID " +
         "  AND ItemID = @ItemID " +
         "  AND BatchCode = @BatchCode " +
         "  AND VchTypeID = @VchTypeID " +
         "  AND CCID = @CCID " +
         "  AND TenantID = @TenantID END " +
         "SELECT " +
         "  @BatchCode END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetItemMaster') " +
                     "DROP PROCEDURE UspGetItemMaster ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "CREATE PROCEDURE [dbo].[UspGetItemMaster] ( @ItemID    NUMERIC   (18,0), @TenantID     NUMERIC   (18,0) )  " +
            "AS  BEGIN 	 " +
            "DECLARE @CatIDsNames	VARCHAR(1000) 	DECLARE @CatIDs	VARCHAR(1000)      IF @ItemID <> 0       BEGIN 	 " +
            "SELECT @CatIDs = CategoryIDs FROM tblItemMaster WHERE ItemID = @ItemID AND TenantID = @TenantID  " +
            "SELECT @CatIDsNames = COALESCE(@CatIDsNames + ',', '') + Category  		FROM tblCategories   " +
            "WHERE TenantID = @TenantID AND ','+ @CatIDs +',' LIKE '%,'+CONVERT(VARCHAR(50),CategoryID)+',%';    " +
            "SELECT I.ItemID,ItemCode,ItemName,I.CategoryID,Description,ISNULL(I.PRate,0) as PRate,ISNULL(SrateCalcMode,0) as SrateCalcMode,CRateAvg,Srate1Per,I.SRate1,Srate2Per, " +
            "I.SRate2,Srate3Per,I.SRate3,I.Srate4,Srate4Per,I.SRate5,Srate5Per,ISNULL(I.MRP,0) as MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID, " +
            "LedgerID,I.UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID, " +
            "PGID,ItemCodeUniCode,UPC,BatchMode,blnExpiry,Qty,MaxQty,IntNoOrWeight,I.SystemName,I.UserID,I.LastUpdateDate,I.LastUpdateTime,I.TenantID,blnCessOnTax,CompCessQty, " +
            "CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,CessPer,VAT,CategoryIDs,ColorIDs,SizeIDs,BrandDisPer,DGroupID,DGroupDisPer,@CatIDsNames as Categories, " +
            "U.UnitShortName as [Unit],ISNULL(BatchCode,0) as BatchCode,BrandID,ISNULL(AltUnitID,0) as AltUnitID,ISNULL(ConvFactor,0) as ConvFactor, " +
            "ISNULL(Shelflife,0) as Shelflife,ISNULL(SRateInclusive,0) as SRateInclusive,ISNULL(PRateInclusive,0) as PRateInclusive,ISNULL(Slabsys,0) as Slabsystem,batchMode, " +
            "ISNULL(DiscPer,0) AS DiscPer,S.BatchUnique, S.StockID,ISNULL(DepartmentID,0) as DepartmentID,ISNULL(CompCessQty,0) as CompCessQty,ISNULL(DefaultExpInDays,0) as DefaultExpInDays " +
            "FROM tblItemMaster I  " +
            "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID  " +
            "LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID 	 " +
            "LEFT JOIN tblStock S ON S.ItemID =I.ItemID     " +
            "WHERE I.ItemID = @ItemID AND I.TenantID = @TenantID 	 END      ELSE      BEGIN  " +
            "SELECT I.ItemID,ItemCode as [Item Code],ItemName as [Item],U.UnitShortName as [Unit],C.Category,Description,I.MRP,HSNID as [HSN Code], " +
            "(CASE WHEN ActiveStatus = 1 THEN 'Active' ELSE 'In Active' END) as Status, 		 ISNULL(BatchCode,0) as BatchCode 		 FROM tblItemMaster I  	 " +
            "INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID 		 LEFT JOIN tblUnit U ON U.UnitID = I.UNITID AND U.TenantID = @TenantID 	 " +
            "LEFT JOIN tblStock S ON S.ItemID = I.ItemID          WHERE I.TenantID = @TenantID      END  END  ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseItemInsert') " +
                     "DROP PROCEDURE UspPurchaseItemInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspPurchaseItemInsert](  " +
                "     @InvID NUMERIC (18, 0),   " +
                "     @ItemId NUMERIC (18, 0),   " +
                "     @Qty FLOAT,   " +
                "     @Rate FLOAT,   " +
                "     @UnitId NUMERIC (18, 0),   " +
                "     @Batch VARCHAR (50),   " +
                "     @TaxPer FLOAT,   " +
                "     @TaxAmount FLOAT,   " +
                "     @Discount FLOAT,   " +
                "     @MRP FLOAT,   " +
                "     @SlNo NUMERIC (18, 0),   " +
                "     @Prate FLOAT,   " +
                "     @Free FLOAT,   " +
                "     @SerialNos VARCHAR (5000),   " +
                "     @ItemDiscount FLOAT,   " +
                "     @BatchCode VARCHAR (50),   " +
                "     @iCessOnTax FLOAT,   " +
                "     @blnCessOnTax NUMERIC (18, 0),   " +
                "     @Expiry DATETIME,   " +
                "     @ItemDiscountPer FLOAT,   " +
                "     @RateInclusive NUMERIC (18, 0),   " +
                "     @ITaxableAmount FLOAT,   " +
                "     @INetAmount FLOAT,   " +
                "     @CGSTTaxPer FLOAT,   " +
                "     @CGSTTaxAmt FLOAT,   " +
                "     @SGSTTaxPer FLOAT,   " +
                "     @SGSTTaxAmt FLOAT,   " +
                "     @IGSTTaxPer FLOAT,   " +
                "     @IGSTTaxAmt FLOAT,   " +
                "     @iRateDiscPer FLOAT,   " +
                "     @iRateDiscount FLOAT,   " +
                "     @BatchUnique VARCHAR (150),   " +
                "     @blnQtyIN NUMERIC (18, 0),   " +
                "     @CRate FLOAT,   " +
                "     @Unit VARCHAR (50),   " +
                "     @ItemStockID NUMERIC (18, 0),   " +
                "     @IcessPercent FLOAT,   " +
                "     @IcessAmt FLOAT,   " +
                "     @IQtyCompCessPer FLOAT,   " +
                "     @IQtyCompCessAmt FLOAT,   " +
                "     @StockMRP FLOAT,   " +
                "     @BaseCRate FLOAT,   " +
                "     @InonTaxableAmount FLOAT,   " +
                "     @IAgentCommPercent FLOAT,   " +
                "     @BlnDelete NUMERIC (18, 0),   " +
                "     @Id NUMERIC (18, 0),   " +
                "     @StrOfferDetails VARCHAR (100),   " +
                "     @BlnOfferItem FLOAT,   " +
                "     @BalQty FLOAT,   " +
                "     @GrossAmount FLOAT,   " +
                "     @iFloodCessPer FLOAT,   " +
                "     @iFloodCessAmt FLOAT,   " +
                "     @Srate1 FLOAT,   " +
                "     @Srate2 FLOAT,   " +
                "     @Srate3 FLOAT,   " +
                "     @Srate4 FLOAT,   " +
                "     @Srate5 FLOAT,   " +
                "     @Costrate FLOAT,   " +
                "     @CostValue FLOAT,   " +
                "     @Profit FLOAT,   " +
                "     @ProfitPer FLOAT,   " +
                "     @DiscMode NUMERIC (18, 0),   " +
                "     @Srate1Per FLOAT,   " +
                "     @Srate2Per FLOAT,   " +
                "     @Srate3Per FLOAT,   " +
                "     @Srate4Per FLOAT,   " +
                "     @Srate5Per FLOAT,   " +
                "     @Action INT = 0  " +
                "   ) AS BEGIN DECLARE @RetResult INT DECLARE @RetID INT DECLARE @VchType VARCHAR(50) DECLARE @VchTypeID NUMERIC(18, 0) DECLARE @BatchMode VARCHAR(50) DECLARE @VchDate DATETIME DECLARE @CCID NUMERIC(18, 0) DECLARE @TenantID NUMERIC(18, 0) DECLARE @BarCode_out VARCHAR(50) DECLARE @VchParentID NUMERIC(18, 0) BEGIN TRY BEGIN TRANSACTION;  " +
                "   SELECT   " +
                "     @VchType = VchType,   " +
                "     @VchTypeID = VchTypeID,   " +
                "     @VchDate = InvDate,   " +
                "     @CCID = CCID,   " +
                "     @TenantID = TenantID   " +
                "   FROM   " +
                "     tblPurchase   " +
                "   WHERE   " +
                "     InvId = @InvID   " +
                "   SELECT   " +
                "     @BatchMode = BatchMode   " +
                "   FROM   " +
                "     tblItemMaster   " +
                "   WHERE   " +
                "     ItemID = @ItemId   " +
                "   SELECT   " +
                "     @VchParentID = ParentID   " +
                "   FROM   " +
                "     tblVchType   " +
                "   WHERE   " +
                "     VchTypeID = @VchTypeID IF @Action = 0 BEGIN IF @VchParentID = 2   " +
                "     BEGIN   " +
                "     EXEC UspTransStockUpdate @ItemId,   " +
                "     @BatchCode,   " +
                "     @BatchUnique,   " +
                "     @Qty,   " +
                "     @MRP,   " +
                "     @CRate,   " +
                "     @CRate,   " +
                "     @Prate,   " +
                "     @Prate,   " +
                "     @TaxPer,   " +
                "     @Srate1,   " +
                "     @Srate2,   " +
                "     @Srate3,   " +
                "     @Srate4,   " +
                "     @Srate5,   " +
                "     @BatchMode,   " +
                "     @VchType,   " +
                "     @VchDate,   " +
                "     @Expiry,   " +
                "     'STOCKADD',   " +
                "     @InvID,   " +
                "     @VchTypeID,   " +
                "     @CCID,   " +
                "     @TenantID,   " +
                "     @Prate,  " +
                "     @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0 BEGIN   " +
                "   SET   " +
                "     @BatchCode = SUBSTRING(  " +
                "       @BarCode_out,   " +
                "       0,   " +
                "       CHARINDEX('@', @BarCode_out)  " +
                "     ) END END ELSE IF @VchParentID = 4   " +
                "     BEGIN   " +
                "     EXEC UspTransStockUpdate @ItemId,   " +
                "     @BatchCode,   " +
                "     @BatchUnique,   " +
                "     @Qty,   " +
                "     @MRP,   " +
                "     @CRate,   " +
                "     @CRate,   " +
                "     @Prate,   " +
                "     @Prate,   " +
                "     @TaxPer,   " +
                "     @Srate1,   " +
                "     @Srate2,   " +
                "     @Srate3,   " +
                "     @Srate4,   " +
                "     @Srate5,   " +
                "     @BatchMode,   " +
                "     @VchType,   " +
                "     @VchDate,   " +
                "     @Expiry,   " +
                "     'STOCKLESS',   " +
                "     @InvID,   " +
                "     @VchTypeID,   " +
                "     @CCID,   " +
                "     @TenantID,   " +
                "     @Prate,  " +
                "     @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0 BEGIN   " +
                "   SET   " +
                "     @BatchCode = SUBSTRING(  " +
                "       @BarCode_out,   " +
                "       0,   " +
                "       CHARINDEX('@', @BarCode_out)  " +
                "     ) END END ELSE IF @VchParentID = 6   " +
                "     BEGIN   " +
                "     EXEC UspTransStockUpdate @ItemId,   " +
                "     @BatchCode,   " +
                "     @BatchUnique,   " +
                "     @Qty,   " +
                "     @MRP,   " +
                "     @CRate,   " +
                "     @CRate,   " +
                "     @Prate,   " +
                "     @Prate,   " +
                "     @TaxPer,   " +
                "     @Srate1,   " +
                "     @Srate2,   " +
                "     @Srate3,   " +
                "     @Srate4,   " +
                "     @Srate5,   " +
                "     @BatchMode,   " +
                "     @VchType,   " +
                "     @VchDate,   " +
                "     @Expiry,   " +
                "     'STOCKADD',   " +
                "     @InvID,   " +
                "     @VchTypeID,   " +
                "     @CCID,   " +
                "     @TenantID,   " +
                "     @Prate,  " +
                "     @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0 BEGIN   " +
                "   SET   " +
                "     @BatchCode = SUBSTRING(  " +
                "       @BarCode_out,   " +
                "       0,   " +
                "       CHARINDEX('@', @BarCode_out)  " +
                "     ) END END ELSE BEGIN   " +
                "   SET   " +
                "     @BatchCode = @BarCode_out END INSERT INTO tblPurchaseItem(  " +
                "       InvID, ItemId, Qty, Rate, UnitId, Batch,   " +
                "       TaxPer, TaxAmount, Discount, MRP,   " +
                "       SlNo, Prate, Free, SerialNos, ItemDiscount,   " +
                "       BatchCode, iCessOnTax, blnCessOnTax,   " +
                "       Expiry, ItemDiscountPer, RateInclusive,   " +
                "       ITaxableAmount, INetAmount, CGSTTaxPer,   " +
                "       CGSTTaxAmt, SGSTTaxPer, SGSTTaxAmt,   " +
                "       IGSTTaxPer, IGSTTaxAmt, iRateDiscPer,   " +
                "       iRateDiscount, BatchUnique, blnQtyIN,   " +
                "       CRate, Unit, ItemStockID, IcessPercent,   " +
                "       IcessAmt, IQtyCompCessPer, IQtyCompCessAmt,   " +
                "       StockMRP, BaseCRate, InonTaxableAmount,   " +
                "       IAgentCommPercent, BlnDelete, StrOfferDetails,   " +
                "       BlnOfferItem, BalQty, GrossAmount,   " +
                "       iFloodCessPer, iFloodCessAmt, Srate1,   " +
                "       Srate2, Srate3, Srate4, Srate5, Costrate,   " +
                "       CostValue, Profit, ProfitPer, DiscMode,   " +
                "       Srate1Per, Srate2Per, Srate3Per,   " +
                "       Srate4Per, Srate5Per  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @InvID, @ItemId, @Qty, @Rate, @UnitId,   " +
                "       @Batch, @TaxPer, @TaxAmount, @Discount,   " +
                "       @MRP, @SlNo, @Prate, @Free, @SerialNos,   " +
                "       @ItemDiscount, @BatchCode, @iCessOnTax,   " +
                "       @blnCessOnTax, @Expiry, @ItemDiscountPer,   " +
                "       @RateInclusive, @ITaxableAmount,   " +
                "       @INetAmount, @CGSTTaxPer, @CGSTTaxAmt,   " +
                "       @SGSTTaxPer, @SGSTTaxAmt, @IGSTTaxPer,   " +
                "       @IGSTTaxAmt, @iRateDiscPer, @iRateDiscount,   " +
                "       @BarCode_out, @blnQtyIN, @CRate,   " +
                "       @Unit, @ItemStockID, @IcessPercent,   " +
                "       @IcessAmt, @IQtyCompCessPer, @IQtyCompCessAmt,   " +
                "       @StockMRP, @BaseCRate, @InonTaxableAmount,   " +
                "       @IAgentCommPercent, @BlnDelete,   " +
                "       @StrOfferDetails, @BlnOfferItem,   " +
                "       @BalQty, @GrossAmount, @iFloodCessPer,   " +
                "       @iFloodCessAmt, @Srate1, @Srate2,   " +
                "       @Srate3, @Srate4, @Srate5, @Costrate,   " +
                "       @CostValue, @Profit, @ProfitPer,   " +
                "       @DiscMode, @Srate1Per, @Srate2Per,   " +
                "       @Srate3Per, @Srate4Per, @Srate5Per  " +
                "     )   " +
                "   SET   " +
                "     @RetResult = 1;  " +
                "   END ELSE IF @Action = 2   " +
                "   BEGIN   " +
                "   EXEC UspTransStockUpdate @ItemId,   " +
                "   @BatchCode,   " +
                "   @BatchUnique,   " +
                "   @Qty,   " +
                "   @MRP,   " +
                "   @CRate,   " +
                "   @CRate,   " +
                "   @Prate,   " +
                "   @Prate,   " +
                "   @TaxPer,   " +
                "   @Srate1,   " +
                "   @Srate2,   " +
                "   @Srate3,   " +
                "   @Srate4,   " +
                "   @Srate5,   " +
                "   @BatchMode,   " +
                "   @VchType,   " +
                "   @VchDate,   " +
                "   @Expiry,   " +
                "   'STOCKDEL',   " +
                "   @InvID,   " +
                "   @VchTypeID,   " +
                "   @CCID,   " +
                "   @TenantID,   " +
                "   @Prate,  " +
                "   @BarCode_out output   " +
                "   DELETE FROM   " +
                "     tblPurchaseItem   " +
                "   WHERE   " +
                "     InvID = @InvID   " +
                "   SET   " +
                "     @RetResult = 0;  " +
                "   END ELSE IF @Action = 3   " +
                "   BEGIN   " +
                "   EXEC UspTransStockUpdate @ItemId,   " +
                "   @BatchCode,   " +
                "   @BatchUnique,   " +
                "   @Qty,   " +
                "   @MRP,   " +
                "   @CRate,   " +
                "   @CRate,   " +
                "   @Prate,   " +
                "   @Prate,   " +
                "   @TaxPer,   " +
                "   @Srate1,   " +
                "   @Srate2,   " +
                "   @Srate3,   " +
                "   @Srate4,   " +
                "   @Srate5,   " +
                "   @BatchMode,   " +
                "   @VchType,   " +
                "   @VchDate,   " +
                "   @Expiry,   " +
                "   'STOCKDEL',   " +
                "   @InvID,   " +
                "   @VchTypeID,   " +
                "   @CCID,   " +
                "   @TenantID,   " +
                "   @Prate,  " +
                "   @BarCode_out output   " +
                "   SET   " +
                "     @RetResult = 0;  " +
                "   END COMMIT TRANSACTION;  " +
                "   SELECT   " +
                "     @RetResult as SqlSpResult,   " +
                "     @RetID as PID END TRY BEGIN CATCH ROLLBACK;  " +
                "   SELECT   " +
                "     -1 as SqlSpResult,   " +
                "     @RetID as PID,   " +
                "     ERROR_NUMBER() AS ErrorNumber,   " +
                "     ERROR_STATE() AS ErrorState,   " +
                "     ERROR_SEVERITY() AS ErrorSeverity,   " +
                "     ERROR_PROCEDURE() AS ErrorProcedure,   " +
                "     ERROR_LINE() AS ErrorLine,   " +
                "     ERROR_MESSAGE() AS ErrorMessage;  " +
                "   END CATCH;  " +
                "   END  ";
                 Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspTransStockUpdate') " +
                     "DROP PROCEDURE UspTransStockUpdate ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspTransStockUpdate] (  " +
                "     @ItemID NUMERIC(18, 0),   " +
                "     @BatchCode VARCHAR(50),   " +
                "     @BatchUniq VARCHAR(50),   " +
                "     @Qty NUMERIC(18, 5),   " +
                "     @MRP NUMERIC(18, 5),   " +
                "     @CostRateInc NUMERIC(18, 5),   " +
                "     @CostRateExcl NUMERIC(18, 5),   " +
                "     @PRateExcl NUMERIC(18, 5),   " +
                "     @PrateInc NUMERIC(18, 5),   " +
                "     @TaxPer NUMERIC(18, 5),   " +
                "     @SRate1 NUMERIC(18, 5),   " +
                "     @SRate2 NUMERIC(18, 5),   " +
                "     @SRate3 NUMERIC(18, 5),   " +
                "     @SRate4 NUMERIC(18, 5),   " +
                "     @SRate5 NUMERIC(18, 5),   " +
                "     @BatchMode INT,   " +
                "     @VchType VARCHAR(100),   " +
                "     @VchDate DATETIME,   " +
                "     @ExpDt DATETIME,   " +
                "     @Action VARCHAR(20),   " +
                "     @RefID NUMERIC(18, 0),   " +
                "     @VchTypeID NUMERIC(18, 0),   " +
                "     @CCID NUMERIC(18, 0),   " +
                "     @TenantID NUMERIC(18, 0),   " +
                "     @ActPrate	NUMERIC(18, 5),   " +
                "     @BarCode_out VARCHAR(50) OUTPUT  " +
                "   ) AS BEGIN DECLARE @BatchID NUMERIC(18, 0) DECLARE @StockID NUMERIC(18, 0) DECLARE @LastInvDt DATETIME = Getdate() DECLARE @STOCKHISID NUMERIC(18, 0) DECLARE @PRFXBATCH VARCHAR(10) DECLARE @Stock NUMERIC(18, 5) DECLARE @INVID NUMERIC(18, 0) DECLARE @BarCode VARCHAR(50) DECLARE @BarUniq VARCHAR(100) DECLARE @CalcQOH NUMERIC(18, 5) DECLARE @BLNADVANCED INT DECLARE @blnExpiry BIT DECLARE @LessQty NUMERIC(18, 5)   " +
                "   SET   " +
                "     @BarCode = @BatchCode   " +
                "   SELECT   " +
                "     @StockID = MAX(  " +
                "       ISNULL(StockID, 0)  " +
                "     ) + 1   " +
                "   FROM   " +
                "     tblStock   " +
                "   WHERE   " +
                "     TenantID = @TenantID   " +
                "   SELECT   " +
                "     @BatchID = MAX(  " +
                "       ISNULL(BatchID, 0)  " +
                "     ) + 1   " +
                "   FROM   " +
                "     tblStock   " +
                "   WHERE   " +
                "     TenantID = @TenantID   " +
                "   SELECT   " +
                "     @STOCKHISID = MAX(  " +
                "       ISNULL(STOCKHISID, 0)  " +
                "     ) + 1   " +
                "   FROM   " +
                "     tblStockHistory   " +
                "   WHERE   " +
                "     TenantID = @TenantID   " +
                "   SELECT   " +
                "     @BLNADVANCED = ISNULl(ValueName, 0)   " +
                "   FROM   " +
                "     [tblAppSettings]   " +
                "   WHERE   " +
                "     UPPER(  " +
                "       LTRIM(  " +
                "         RTRIM(KeyName)  " +
                "       )  " +
                "     ) = 'BLNADVANCED'   " +
                "   SELECT   " +
                "     @Stock = ISNULL(QOH, 0)   " +
                "   FROM   " +
                "     tblStock   " +
                "   WHERE   " +
                "     ItemID = @ItemID   " +
                "     AND BatchCode = @BatchCode   " +
                "     AND TenantID = @TenantID   " +
                "   SELECT   " +
                "     @blnExpiry = ISNULL(blnExpiry, 0)   " +
                "   FROM   " +
                "     tblItemMaster   " +
                "   WHERE   " +
                "     ItemID = @ItemID   " +
                "   SET   " +
                "     @LessQty = -1 IF @StockID = 0 BEGIN   " +
                "   SET   " +
                "     @StockID = 1 END IF @STOCKHISID = 0 BEGIN   " +
                "   SET   " +
                "     @STOCKHISID = 1 END IF @BatchID = 0 BEGIN   " +
                "   SET   " +
                "     @BatchID = 1 END IF @Action = 'STOCKADD' BEGIN IF @BatchCode = '<Auto Barcode>' BEGIN Declare @Prefix VARCHAR(50) Declare @BatchPrefix VARCHAR(50) IF @BLNADVANCED = 1 BEGIN   " +
                "   Select   " +
                "     @BatchPrefix = ValueName   " +
                "   from   " +
                "     tblAppSettings   " +
                "   where   " +
                "     KeyName = 'STRBATCODEPREFIXSUFFIX'   " +
                "   set   " +
                "     @BatchPrefix = (  " +
                "       SELECT   " +
                "         PARSENAME(  " +
                "           REPLACE(@BatchPrefix, 'ƒ', ''),   " +
                "           1  " +
                "         )  " +
                "     ) IF(@BatchPrefix = '<YEARMONTH>') BEGIN   " +
                "   SELECT   " +
                "     @Prefix =(  " +
                "       Select   " +
                "         [dbo].[UfnBatchCodePrefixSuffix](@BatchPrefix)  " +
                "     )   " +
                "   SET   " +
                "     @BatchCode = @Prefix + CONVERT(VARCHAR, @BatchID) END ELSE BEGIN   " +
                "   Select   " +
                "     @BatchPrefix = ValueName   " +
                "   from   " +
                "     tblAppSettings   " +
                "   where   " +
                "     KeyName = 'STRBATCODEPREFIXSUFFIX'   " +
                "   set   " +
                "     @BatchPrefix = (  " +
                "       SELECT   " +
                "         PARSENAME(  " +
                "           REPLACE(@BatchPrefix, 'ƒ', ''),   " +
                "           1  " +
                "         )  " +
                "     )   " +
                "   SELECT   " +
                "     @BatchCode = ISNULL(  " +
                "       MAX(BatchID),   " +
                "       0  " +
                "     ) + 1   " +
                "   FROM   " +
                "     tblStock   " +
                "   WHERE   " +
                "     TenantID = @TenantID IF @BatchPrefix <> '' BEGIN   " +
                "   SET   " +
                "     @BatchCode = @BatchPrefix + CONVERT(VARCHAR, @BatchID) END END END ELSE BEGIN   " +
                "   SELECT   " +
                "     @BatchCode = ISNULL(  " +
                "       MAX(BatchID),   " +
                "       0  " +
                "     ) + 1   " +
                "   FROM   " +
                "     tblStock   " +
                "   WHERE   " +
                "     TenantID = @TenantID END   " +
                "   SET   " +
                "     @BarCode = @BatchCode IF @blnExpiry = 1 BEGIN   " +
                "   SET   " +
                "     @BatchUniq = @BatchCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) + '@' + REPLACE(  " +
                "       CONVERT(  " +
                "         VARCHAR(10),   " +
                "         FORMAT(@ExpDt, 'dd-MM-yy')  " +
                "       ),   " +
                "       '-',   " +
                "       ''  " +
                "     )   " +
                "   SET   " +
                "     @BarUniq = @BarCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) + '@' + REPLACE(  " +
                "       CONVERT(  " +
                "         VARCHAR(10),   " +
                "         FORMAT(@ExpDt, 'dd-MM-yy')  " +
                "       ),   " +
                "       '-',   " +
                "       ''  " +
                "     ) END ELSE BEGIN   " +
                "   SET   " +
                "     @BatchUniq = @BatchCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     )   " +
                "   SET   " +
                "     @BarUniq = @BarCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) END IF EXISTS(  " +
                "       SELECT   " +
                "         *   " +
                "       FROM   " +
                "         tblStock   " +
                "       WHERE   " +
                "         ItemID = @ItemID   " +
                "         AND BatchCode = @BarCode   " +
                "         AND TenantID = @TenantID   " +
                "         AND BatchUnique = @BatchUniq   " +
                "         AND CCID = @CCID  " +
                "     ) BEGIN IF @VchTypeID <> 0 BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BarCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     1,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate   " +
                "     INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BarCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END ELSE BEGIN IF EXISTS(  " +
                "       SELECT   " +
                "         *   " +
                "       FROM   " +
                "         tblStock   " +
                "       WHERE   " +
                "         ItemID = @ItemID   " +
                "         AND BatchCode = @BarCode   " +
                "         AND TenantID = @TenantID   " +
                "         AND CCID = @CCID  " +
                "     ) BEGIN IF @VchTypeID <> 0   " +
                "     BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BarCode,   " +
                "     @BarUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     0,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate  " +
                "     INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BarCode, @BarUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END ELSE BEGIN IF @VchTypeID <> 0   " +
                "     BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BatchCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     0,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate  " +
                "     INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BatchCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END END END ELSE BEGIN IF @BatchMode = 0 BEGIN   " +
                "   SELECT   " +
                "     @BatchCode = ItemCode   " +
                "   FROM   " +
                "     tblItemMaster   " +
                "   WHERE   " +
                "     ItemID = @ItemID   " +
                "   SET   " +
                "     @BarCode = @BatchCode IF EXISTS(  " +
                "       SELECT   " +
                "         *   " +
                "       FROM   " +
                "         tblStock   " +
                "       WHERE   " +
                "         ItemID = @ItemID   " +
                "         AND BatchCode = @BarCode   " +
                "         AND TenantID = @TenantID   " +
                "         AND BatchUnique = @BatchUniq   " +
                "         AND CCID = @CCID  " +
                "     )   " +
                "     BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BatchCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     1,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate  " +
                "     IF @VchTypeID <> 0 BEGIN INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BatchCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END ELSE   " +
                "     BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BatchCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     0,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate  " +
                "     IF @VchTypeID <> 0 BEGIN INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BatchCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END END ELSE BEGIN IF CHARINDEX('@', @BatchUniq) = 0 BEGIN IF @blnExpiry = 1 BEGIN   " +
                "   SET   " +
                "     @BatchUniq = @BatchCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) + '@' + REPLACE(  " +
                "       CONVERT(  " +
                "         VARCHAR(10),   " +
                "         FORMAT(@ExpDt, 'dd-MM-yy')  " +
                "       ),   " +
                "       '-',   " +
                "       ''  " +
                "     ) END ELSE BEGIN   " +
                "   SET   " +
                "     @BatchUniq = @BatchCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) END END ELSE BEGIN IF @blnExpiry = 1 BEGIN   " +
                "   SET   " +
                "     @BatchUniq = @BatchCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) + '@' + REPLACE(  " +
                "       CONVERT(  " +
                "         VARCHAR(10),   " +
                "         FORMAT(@ExpDt, 'dd-MM-yy')  " +
                "       ),   " +
                "       '-',   " +
                "       ''  " +
                "     ) END ELSE BEGIN   " +
                "   SET   " +
                "     @BatchUniq = @BatchCode + '@' + CONVERT(  " +
                "       VARCHAR(22),   " +
                "       CONVERT(  " +
                "         NUMERIC(18, 2),   " +
                "         @MRP  " +
                "       )  " +
                "     ) END END IF EXISTS(  " +
                "       SELECT   " +
                "         *   " +
                "       FROM   " +
                "         tblStock   " +
                "       WHERE   " +
                "         ItemID = @ItemID   " +
                "         AND BatchCode = @BarCode   " +
                "         AND TenantID = @TenantID   " +
                "         AND BatchUnique = @BatchUniq   " +
                "         AND CCID = @CCID   " +
                "         AND MRP = @MRP   " +
                "         AND FORMAT(ExpiryDate, 'dd-MM-yy') = FORMAT(@ExpDt, 'dd-MM-yy')  " +
                "     )   " +
                "     BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BatchCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     1,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate  " +
                "     IF @VchTypeID <> 0 BEGIN INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BatchCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END ELSE   " +
                "     BEGIN   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BatchCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @Qty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     0,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate  " +
                "     IF @VchTypeID <> 0 BEGIN INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       @Qty, 0, @BatchCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END END END   " +
                "   SET   " +
                "     @BatchCode = @BarCode END IF @Action = 'STOCKLESS' BEGIN IF EXISTS(  " +
                "       SELECT   " +
                "         *   " +
                "       FROM   " +
                "         tblStock   " +
                "       WHERE   " +
                "         ItemID = @ItemID   " +
                "         AND BatchCode = @BarCode   " +
                "         AND TenantID = @TenantID   " +
                "         AND BatchUnique = @BatchUniq   " +
                "         AND CCID = @CCID  " +
                "     ) BEGIN IF @VchTypeID <> 0 BEGIN   " +
                "   SET   " +
                "     @LessQty = @LessQty * @Qty   " +
                "     EXEC UspStockInsert @StockID,   " +
                "     @TenantID,   " +
                "     @CCID,   " +
                "     @BarCode,   " +
                "     @BatchUniq,   " +
                "     @BatchID,   " +
                "     @MRP,   " +
                "     @ExpDt,   " +
                "     @CostRateInc,   " +
                "     @CostRateExcl,   " +
                "     @PRateExcl,   " +
                "     @PrateInc,   " +
                "     @TaxPer,   " +
                "     @SRate1,   " +
                "     @SRate2,   " +
                "     @SRate3,   " +
                "     @SRate4,   " +
                "     @SRate5,   " +
                "     @LessQty,   " +
                "     @LastInvDt,   " +
                "     '',   " +
                "     NULL,   " +
                "     1,   " +
                "     @ItemID,   " +
                "     @BatchMode,  " +
                "     @ActPrate   " +
                "     INSERT INTO tblStockHistory(  " +
                "       VchType, VchDate, RefId, ItemID, QtyIn,   " +
                "       QtyOut, BatchCode, BatchUnique, Expiry,   " +
                "       MRP, CostRateInc, CostRateExcl, PRateExcl,   " +
                "       PrateInc, TaxPer, SRate1, SRate2,   " +
                "       SRate3, SRate4, SRate5, VchTypeID,   " +
                "       CCID, STOCKHISID, TenantID, StockID  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @VchType, @VchDate, @RefID, @ItemID,   " +
                "       0, @Qty, @BarCode, @BatchUniq, @ExpDt,   " +
                "       @MRP, @CostRateInc, @CostRateExcl,   " +
                "       @PRateExcl, @PrateInc, @TaxPer, @SRate1,   " +
                "       @SRate2, @SRate3, @SRate4, @SRate5,   " +
                "       @VchTypeID, @CCID, @STOCKHISID, @TenantID,   " +
                "       @StockID  " +
                "     ) END END END IF @Action = 'STOCKDEL' BEGIN   " +
                "   SELECT   " +
                "     @CalcQOH = QOH   " +
                "   FROM   " +
                "     tblStock   " +
                "   WHERE   " +
                "     ItemID = @ItemID   " +
                "     AND CCID = @CCID   " +
                "     AND BatchUnique = @BatchUniq   " +
                "     AND TenantID = @TenantID   " +
                "   UPDATE   " +
                "     tblStock   " +
                "   SET   " +
                "     QOH = QOH - @CalcQOH   " +
                "   WHERE   " +
                "     ItemID = @ItemID   " +
                "     AND CCID = @CCID   " +
                "     AND BatchUnique = @BatchUniq   " +
                "     AND TenantID = @TenantID   " +
                "   DELETE FROM   " +
                "     tblStockHistory   " +
                "   WHERE   " +
                "     RefId = @RefID   " +
                "     AND CCID = @CCID   " +
                "     AND TenantID = @TenantID END   " +
                "   SET   " +
                "     @BarCode_out = @BatchUniq   " +
                "   SELECT   " +
                "     @BarCode_out END  ";
                 Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseItemInsert') " +
                     "DROP PROCEDURE UspPurchaseItemInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspPurchaseItemInsert]  " +
                "   (  " +
                "     @InvID NUMERIC (18, 0),   " +
                "     @ItemId NUMERIC (18, 0),   " +
                "     @Qty FLOAT,   " +
                "     @Rate FLOAT,   " +
                "     @UnitId NUMERIC (18, 0),   " +
                "     @Batch VARCHAR (50),   " +
                "     @TaxPer FLOAT,   " +
                "     @TaxAmount FLOAT,   " +
                "     @Discount FLOAT,   " +
                "     @MRP FLOAT,   " +
                "     @SlNo NUMERIC (18, 0),   " +
                "     @Prate FLOAT,   " +
                "     @Free FLOAT,   " +
                "     @SerialNos VARCHAR (5000),   " +
                "     @ItemDiscount FLOAT,   " +
                "     @BatchCode VARCHAR (50),   " +
                "     @iCessOnTax FLOAT,   " +
                "     @blnCessOnTax NUMERIC (18, 0),   " +
                "     @Expiry DATETIME,   " +
                "     @ItemDiscountPer FLOAT,   " +
                "     @RateInclusive NUMERIC (18, 0),   " +
                "     @ITaxableAmount FLOAT,   " +
                "     @INetAmount FLOAT,   " +
                "     @CGSTTaxPer FLOAT,   " +
                "     @CGSTTaxAmt FLOAT,   " +
                "     @SGSTTaxPer FLOAT,   " +
                "     @SGSTTaxAmt FLOAT,   " +
                "     @IGSTTaxPer FLOAT,   " +
                "     @IGSTTaxAmt FLOAT,   " +
                "     @iRateDiscPer FLOAT,   " +
                "     @iRateDiscount FLOAT,   " +
                "     @BatchUnique VARCHAR (150),   " +
                "     @blnQtyIN NUMERIC (18, 0),   " +
                "     @CRate FLOAT,   " +
                "     @Unit VARCHAR (50),   " +
                "     @ItemStockID NUMERIC (18, 0),   " +
                "     @IcessPercent FLOAT,   " +
                "     @IcessAmt FLOAT,   " +
                "     @IQtyCompCessPer FLOAT,   " +
                "     @IQtyCompCessAmt FLOAT,   " +
                "     @StockMRP FLOAT,   " +
                "     @BaseCRate FLOAT,   " +
                "     @InonTaxableAmount FLOAT,   " +
                "     @IAgentCommPercent FLOAT,   " +
                "     @BlnDelete NUMERIC (18, 0),   " +
                "     @Id NUMERIC (18, 0),   " +
                "     @StrOfferDetails VARCHAR (100),   " +
                "     @BlnOfferItem FLOAT,   " +
                "     @BalQty FLOAT,   " +
                "     @GrossAmount FLOAT,   " +
                "     @iFloodCessPer FLOAT,   " +
                "     @iFloodCessAmt FLOAT,   " +
                "     @Srate1 FLOAT,   " +
                "     @Srate2 FLOAT,   " +
                "     @Srate3 FLOAT,   " +
                "     @Srate4 FLOAT,   " +
                "     @Srate5 FLOAT,   " +
                "     @Costrate FLOAT,   " +
                "     @CostValue FLOAT,   " +
                "     @Profit FLOAT,   " +
                "     @ProfitPer FLOAT,   " +
                "     @DiscMode NUMERIC (18, 0),   " +
                "     @Srate1Per FLOAT,   " +
                "     @Srate2Per FLOAT,   " +
                "     @Srate3Per FLOAT,   " +
                "     @Srate4Per FLOAT,   " +
                "     @Srate5Per FLOAT,   " +
                "     @Action INT = 0  " +
                "   )   " +
                "   AS   " +
                "   BEGIN   " +
                "   DECLARE @RetResult INT DECLARE @RetID INT DECLARE @VchType VARCHAR(50) DECLARE @VchTypeID NUMERIC(18, 0) DECLARE @BatchMode VARCHAR(50) DECLARE @VchDate DATETIME DECLARE @CCID NUMERIC(18, 0) DECLARE @TenantID NUMERIC(18, 0) DECLARE @BarCode_out VARCHAR(50) DECLARE @VchParentID NUMERIC(18, 0)   " +
                "   DECLARE @FreeQty NUMERIC(18, 0)   " +
                "   BEGIN TRY   " +
                "   BEGIN TRANSACTION;  " +
                "   SET @FreeQty = @Qty + @Free  " +
                "   SELECT   " +
                "     @VchType = VchType,   " +
                "     @VchTypeID = VchTypeID,   " +
                "     @VchDate = InvDate,   " +
                "     @CCID = CCID,   " +
                "     @TenantID = TenantID   " +
                "   FROM   " +
                "     tblPurchase   " +
                "   WHERE   " +
                "     InvId = @InvID   " +
                "   SELECT   " +
                "     @BatchMode = BatchMode   " +
                "   FROM   " +
                "     tblItemMaster   " +
                "   WHERE   " +
                "     ItemID = @ItemId   " +
                "   SELECT   " +
                "     @VchParentID = ParentID   " +
                "   FROM   " +
                "     tblVchType   " +
                "   WHERE   " +
                "     VchTypeID = @VchTypeID IF @Action = 0 BEGIN IF @VchParentID = 2   " +
                "     BEGIN   " +
                "     EXEC UspTransStockUpdate @ItemId,   " +
                "     @BatchCode,   " +
                "     @BatchUnique,   " +
                "     @FreeQty,   " +
                "     @MRP,   " +
                "     @CRate,   " +
                "     @CRate,   " +
                "     @Prate,   " +
                "     @Prate,   " +
                "     @TaxPer,   " +
                "     @Srate1,   " +
                "     @Srate2,   " +
                "     @Srate3,   " +
                "     @Srate4,   " +
                "     @Srate5,   " +
                "     @BatchMode,   " +
                "     @VchType,   " +
                "     @VchDate,   " +
                "     @Expiry,   " +
                "     'STOCKADD',   " +
                "     @InvID,   " +
                "     @VchTypeID,   " +
                "     @CCID,   " +
                "     @TenantID,   " +
                "     @Prate,  " +
                "     @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0 BEGIN   " +
                "   SET   " +
                "     @BatchCode = SUBSTRING(  " +
                "       @BarCode_out,   " +
                "       0,   " +
                "       CHARINDEX('@', @BarCode_out)  " +
                "     ) END END ELSE IF @VchParentID = 4   " +
                "     BEGIN   " +
                "     EXEC UspTransStockUpdate @ItemId,   " +
                "     @BatchCode,   " +
                "     @BatchUnique,   " +
                "     @FreeQty,   " +
                "     @MRP,   " +
                "     @CRate,   " +
                "     @CRate,   " +
                "     @Prate,   " +
                "     @Prate,   " +
                "     @TaxPer,   " +
                "     @Srate1,   " +
                "     @Srate2,   " +
                "     @Srate3,   " +
                "     @Srate4,   " +
                "     @Srate5,   " +
                "     @BatchMode,   " +
                "     @VchType,   " +
                "     @VchDate,   " +
                "     @Expiry,   " +
                "     'STOCKLESS',   " +
                "     @InvID,   " +
                "     @VchTypeID,   " +
                "     @CCID,   " +
                "     @TenantID,   " +
                "     @Prate,  " +
                "     @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0 BEGIN   " +
                "   SET   " +
                "     @BatchCode = SUBSTRING(  " +
                "       @BarCode_out,   " +
                "       0,   " +
                "       CHARINDEX('@', @BarCode_out)  " +
                "     ) END END ELSE IF @VchParentID = 6   " +
                "     BEGIN   " +
                "     EXEC UspTransStockUpdate @ItemId,   " +
                "     @BatchCode,   " +
                "     @BatchUnique,   " +
                "     @FreeQty,   " +
                "     @MRP,   " +
                "     @CRate,   " +
                "     @CRate,   " +
                "     @Prate,   " +
                "     @Prate,   " +
                "     @TaxPer,   " +
                "     @Srate1,   " +
                "     @Srate2,   " +
                "     @Srate3,   " +
                "     @Srate4,   " +
                "     @Srate5,   " +
                "     @BatchMode,   " +
                "     @VchType,   " +
                "     @VchDate,   " +
                "     @Expiry,   " +
                "     'STOCKADD',   " +
                "     @InvID,   " +
                "     @VchTypeID,   " +
                "     @CCID,   " +
                "     @TenantID,   " +
                "     @Prate,  " +
                "     @BarCode_out output IF CHARINDEX('@', @BarCode_out) > 0 BEGIN   " +
                "   SET   " +
                "     @BatchCode = SUBSTRING(  " +
                "       @BarCode_out,   " +
                "       0,   " +
                "       CHARINDEX('@', @BarCode_out)  " +
                "     ) END END ELSE BEGIN   " +
                "   SET   " +
                "     @BatchCode = @BarCode_out END INSERT INTO tblPurchaseItem(  " +
                "       InvID, ItemId, Qty, Rate, UnitId, Batch,   " +
                "       TaxPer, TaxAmount, Discount, MRP,   " +
                "       SlNo, Prate, Free, SerialNos, ItemDiscount,   " +
                "       BatchCode, iCessOnTax, blnCessOnTax,   " +
                "       Expiry, ItemDiscountPer, RateInclusive,   " +
                "       ITaxableAmount, INetAmount, CGSTTaxPer,   " +
                "       CGSTTaxAmt, SGSTTaxPer, SGSTTaxAmt,   " +
                "       IGSTTaxPer, IGSTTaxAmt, iRateDiscPer,   " +
                "       iRateDiscount, BatchUnique, blnQtyIN,   " +
                "       CRate, Unit, ItemStockID, IcessPercent,   " +
                "       IcessAmt, IQtyCompCessPer, IQtyCompCessAmt,   " +
                "       StockMRP, BaseCRate, InonTaxableAmount,   " +
                "       IAgentCommPercent, BlnDelete, StrOfferDetails,   " +
                "       BlnOfferItem, BalQty, GrossAmount,   " +
                "       iFloodCessPer, iFloodCessAmt, Srate1,   " +
                "       Srate2, Srate3, Srate4, Srate5, Costrate,   " +
                "       CostValue, Profit, ProfitPer, DiscMode,   " +
                "       Srate1Per, Srate2Per, Srate3Per,   " +
                "       Srate4Per, Srate5Per  " +
                "     )   " +
                "   VALUES   " +
                "     (  " +
                "       @InvID, @ItemId, @Qty, @Rate, @UnitId,   " +
                "       @Batch, @TaxPer, @TaxAmount, @Discount,   " +
                "       @MRP, @SlNo, @Prate, @Free, @SerialNos,   " +
                "       @ItemDiscount, @BatchCode, @iCessOnTax,   " +
                "       @blnCessOnTax, @Expiry, @ItemDiscountPer,   " +
                "       @RateInclusive, @ITaxableAmount,   " +
                "       @INetAmount, @CGSTTaxPer, @CGSTTaxAmt,   " +
                "       @SGSTTaxPer, @SGSTTaxAmt, @IGSTTaxPer,   " +
                "       @IGSTTaxAmt, @iRateDiscPer, @iRateDiscount,   " +
                "       @BarCode_out, @blnQtyIN, @CRate,   " +
                "       @Unit, @ItemStockID, @IcessPercent,   " +
                "       @IcessAmt, @IQtyCompCessPer, @IQtyCompCessAmt,   " +
                "       @StockMRP, @BaseCRate, @InonTaxableAmount,   " +
                "       @IAgentCommPercent, @BlnDelete,   " +
                "       @StrOfferDetails, @BlnOfferItem,   " +
                "       @BalQty, @GrossAmount, @iFloodCessPer,   " +
                "       @iFloodCessAmt, @Srate1, @Srate2,   " +
                "       @Srate3, @Srate4, @Srate5, @Costrate,   " +
                "       @CostValue, @Profit, @ProfitPer,   " +
                "       @DiscMode, @Srate1Per, @Srate2Per,   " +
                "       @Srate3Per, @Srate4Per, @Srate5Per  " +
                "     )   " +
                "   SET   " +
                "     @RetResult = 1;  " +
                "   END ELSE IF @Action = 2   " +
                "   BEGIN   " +
                "   EXEC UspTransStockUpdate @ItemId,   " +
                "   @BatchCode,   " +
                "   @BatchUnique,   " +
                "   @FreeQty,   " +
                "   @MRP,   " +
                "   @CRate,   " +
                "   @CRate,   " +
                "   @Prate,   " +
                "   @Prate,   " +
                "   @TaxPer,   " +
                "   @Srate1,   " +
                "   @Srate2,   " +
                "   @Srate3,   " +
                "   @Srate4,   " +
                "   @Srate5,   " +
                "   @BatchMode,   " +
                "   @VchType,   " +
                "   @VchDate,   " +
                "   @Expiry,   " +
                "   'STOCKDEL',   " +
                "   @InvID,   " +
                "   @VchTypeID,   " +
                "   @CCID,   " +
                "   @TenantID,   " +
                "   @Prate,  " +
                "   @BarCode_out output   " +
                "   DELETE FROM   " +
                "     tblPurchaseItem   " +
                "   WHERE   " +
                "     InvID = @InvID   " +
                "   SET   " +
                "     @RetResult = 0;  " +
                "   END ELSE IF @Action = 3   " +
                "   BEGIN   " +
                "   EXEC UspTransStockUpdate @ItemId,   " +
                "   @BatchCode,   " +
                "   @BatchUnique,   " +
                "   @FreeQty,   " +
                "   @MRP,   " +
                "   @CRate,   " +
                "   @CRate,   " +
                "   @Prate,   " +
                "   @Prate,   " +
                "   @TaxPer,   " +
                "   @Srate1,   " +
                "   @Srate2,   " +
                "   @Srate3,   " +
                "   @Srate4,   " +
                "   @Srate5,   " +
                "   @BatchMode,   " +
                "   @VchType,   " +
                "   @VchDate,   " +
                "   @Expiry,   " +
                "   'STOCKDEL',   " +
                "   @InvID,   " +
                "   @VchTypeID,   " +
                "   @CCID,   " +
                "   @TenantID,   " +
                "   @Prate,  " +
                "   @BarCode_out output   " +
                "   SET   " +
                "     @RetResult = 0;  " +
                "   END COMMIT TRANSACTION;  " +
                "   SELECT   " +
                "     @RetResult as SqlSpResult,   " +
                "     @RetID as PID END TRY BEGIN CATCH ROLLBACK;  " +
                "   SELECT   " +
                "     -1 as SqlSpResult,   " +
                "     @RetID as PID,   " +
                "     ERROR_NUMBER() AS ErrorNumber,   " +
                "     ERROR_STATE() AS ErrorState,   " +
                "     ERROR_SEVERITY() AS ErrorSeverity,   " +
                "     ERROR_PROCEDURE() AS ErrorProcedure,   " +
                "     ERROR_LINE() AS ErrorLine,   " +
                "     ERROR_MESSAGE() AS ErrorMessage;  " +
                "   END CATCH;  " +
                "   END  ";
                 Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'Uspgetstockreport') " +
                     "DROP PROCEDURE Uspgetstockreport ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE[dbo].[Uspgetstockreport] (  " +
                "     @ToDate DATETIME,   " +
                "     @TenantID NUMERIC(18, 0)  " +
                "   ) AS BEGIN DECLARE @CurrencyDecimal NUMERIC(18, 0) = (  " +
                "     SELECT   " +
                "       valuename   " +
                "     FROM   " +
                "       tblappsettings   " +
                "     WHERE   " +
                "       keyname = 'CurrencyDecimals'  " +
                "   ) DECLARE @QtyDecimal NUMERIC(18, 0) = (  " +
                "     SELECT   " +
                "       valuename   " +
                "     FROM   " +
                "       tblappsettings   " +
                "     WHERE   " +
                "       keyname = 'QtyDecimalFormat'  " +
                "   )   " +
                "   SELECT   " +
                "     ItemName as [Item Name],   " +
                "     SUM(  " +
                "       Round(  " +
                "         Isnull(qtyin, 0),   " +
                "         @QtyDecimal  " +
                "       )  " +
                "     ) AS[Qty In],   " +
                "     SUM(  " +
                "       Round(  " +
                "         Isnull(qtyout, 0),   " +
                "         @QtyDecimal  " +
                "       )  " +
                "     ) AS[Qty Out],   " +
                "     (  " +
                "       SUM(  " +
                "         Round(  " +
                "           Isnull(qtyin, 0),   " +
                "           @QtyDecimal  " +
                "         )  " +
                "       ) - SUM(  " +
                "         Round(  " +
                "           Isnull(qtyout, 0),   " +
                "           @QtyDecimal  " +
                "         )  " +
                "       )  " +
                "     ) as [QOH],   " +
                "     H.ItemID   " +
                "   FROM   " +
                "     tblStockHistory H  " +
                "     INNER JOIN tblItemMaster I ON I.ItemID = H.ItemID   " +
                "   WHERE   " +
                "     H.TenantID = @TenantID   " +
                "     AND CONVERT(DATETIME, H.VchDate, 106) <= @ToDate   " +
                "   GROUP BY   " +
                "     H.ItemID,   " +
                "     ItemName END  ";
                 Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockDetails') " +
                     "DROP PROCEDURE UspGetStockDetails ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspGetStockDetails]  " +
                "   (  " +
                "   	@StockID		NUMERIC(18,0),  " +
                "   	@BatchCode		VARCHAR(50),  " +
                "   	@TenantID		NUMERIC(18,0),  " +
                "   	@ItemID			NUMERIC(18,0),  " +
                "   	@CCID			NUMERIC(18,0) = 1,  " +
                "   	@BatchUnique	VARCHAR(50)  " +
                "   )  " +
                "   AS  " +
                "   BEGIN  " +
                "   	IF @StockID <> 0  " +
                "   	BEGIN  " +
                "   		SELECT StockID,CCID,BatchCode,BatchUnique,BatchID,MRP,ExpiryDate,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,  " +
                "   		SRate1,SRate2,SRate3,SRate4,SRate5,QOH,LastInvDate,LastInvNo,LastSupplierID,ItemID,ISNULL(PRate,0) as PRate FROM tblStock   " +
                "   		WHERE StockID = @StockID AND TenantID = @TenantID   " +
                "   	END  " +
                "   	ELSE  " +
                "   	BEGIN  " +
                "   		IF @BatchUnique <> ''  " +
                "   		BEGIN  " +
                "   				SELECT StockID,CCID,BatchCode,BatchUnique,BatchID,MRP,ExpiryDate,CostRateInc,CostRateExcl,PRateExcl,PrateInc,TaxPer,  " +
                "   				SRate1,SRate2,SRate3,SRate4,SRate5,QOH,LastInvDate,LastInvNo,LastSupplierID,ItemID,ISNULL(PRate,0) as PRate FROM tblStock   " +
                "   				WHERE BatchUnique = @BatchUnique AND TenantID = @TenantID AND ItemID =  @ItemID" +
                "   		END  " +
                "   	END  " +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspStockInsert') " +
                    "DROP PROCEDURE UspStockInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspStockInsert] (  " + System.Environment.NewLine +
                "   	@StockID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@TenantID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@CCID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@BatchCode VARCHAR(100)  " + System.Environment.NewLine +
                "   	,@BatchUnique VARCHAR(50)  " + System.Environment.NewLine +
                "   	,@BatchID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@MRP NUMERIC(18, 5)  " + System.Environment.NewLine +
                "   	,@ExpiryDate DATE  " + System.Environment.NewLine +
                "   	,@CostRateInc DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@CostRateExcl DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@PRateExcl DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@PrateInc DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@TaxPer DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@SRate1 DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@SRate2 DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@SRate3 DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@SRate4 DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@SRate5 DECIMAL(18, 2)  " + System.Environment.NewLine +
                "   	,@QOH DECIMAL  " + System.Environment.NewLine +
                "   	,@LastInvDate DATE  " + System.Environment.NewLine +
                "   	,@LastInvNo VARCHAR(50)  " + System.Environment.NewLine +
                "   	,@LastSupplierID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Action INT = 0  " + System.Environment.NewLine +
                "   	,@ItemID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@BatchMode VARCHAR(100)  " + System.Environment.NewLine +
                "   	,@PRate NUMERIC(18, 5)  " + System.Environment.NewLine +
                "   	)  " + System.Environment.NewLine +
                "   AS  " + System.Environment.NewLine +
                "   BEGIN  " + System.Environment.NewLine +
                "   	DECLARE @RetResult INT  " + System.Environment.NewLine +
                "   	DECLARE @TransType CHAR(1)  " + System.Environment.NewLine +
                "   	DECLARE @blnExpiry NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	BEGIN TRY  " + System.Environment.NewLine +
                "   		BEGIN TRANSACTION;  " + System.Environment.NewLine +
                "   		IF @BatchMode = 0  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			SELECT @BatchCode = ItemCode  " + System.Environment.NewLine +
                "   				,@blnExpiry = ISNULL(blnExpiry, 0)  " + System.Environment.NewLine +
                "   			FROM tblItemMaster  " + System.Environment.NewLine +
                "   			WHERE ItemID = @ItemID  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		IF @Action = 0  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			IF @BatchMode = 0  " + System.Environment.NewLine +
                "   			BEGIN /*None*/  " + System.Environment.NewLine +
                "   				INSERT INTO tblStock (  " + System.Environment.NewLine +
                "   					StockID  " + System.Environment.NewLine +
                "   					,TenantID  " + System.Environment.NewLine +
                "   					,CCID  " + System.Environment.NewLine +
                "   					,BatchCode  " + System.Environment.NewLine +
                "   					,BatchUnique  " + System.Environment.NewLine +
                "   					,BatchID  " + System.Environment.NewLine +
                "   					,MRP  " + System.Environment.NewLine +
                "   					,ExpiryDate  " + System.Environment.NewLine +
                "   					,CostRateInc  " + System.Environment.NewLine +
                "   					,CostRateExcl  " + System.Environment.NewLine +
                "   					,PRateExcl  " + System.Environment.NewLine +
                "   					,PrateInc  " + System.Environment.NewLine +
                "   					,TaxPer  " + System.Environment.NewLine +
                "   					,SRate1  " + System.Environment.NewLine +
                "   					,SRate2  " + System.Environment.NewLine +
                "   					,SRate3  " + System.Environment.NewLine +
                "   					,SRate4  " + System.Environment.NewLine +
                "   					,SRate5  " + System.Environment.NewLine +
                "   					,QOH  " + System.Environment.NewLine +
                "   					,LastInvDate  " + System.Environment.NewLine +
                "   					,LastInvNo  " + System.Environment.NewLine +
                "   					,LastSupplierID  " + System.Environment.NewLine +
                "   					,ItemID  " + System.Environment.NewLine +
                "   					,PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				VALUES (  " + System.Environment.NewLine +
                "   					@StockID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@BatchID  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@ExpiryDate  " + System.Environment.NewLine +
                "   					,@CostRateInc  " + System.Environment.NewLine +
                "   					,@CostRateExcl  " + System.Environment.NewLine +
                "   					,@PRateExcl  " + System.Environment.NewLine +
                "   					,@PrateInc  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@SRate1  " + System.Environment.NewLine +
                "   					,@SRate2  " + System.Environment.NewLine +
                "   					,@SRate3  " + System.Environment.NewLine +
                "   					,@SRate4  " + System.Environment.NewLine +
                "   					,@SRate5  " + System.Environment.NewLine +
                "   					,ABS(@QOH)  " + System.Environment.NewLine +
                "   					,@LastInvDate  " + System.Environment.NewLine +
                "   					,@LastInvNo  " + System.Environment.NewLine +
                "   					,@LastSupplierID  " + System.Environment.NewLine +
                "   					,@ItemID  " + System.Environment.NewLine +
                "   					,@PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				SET @RetResult = 1;  " + System.Environment.NewLine +
                "   				SET @TransType = 'S';  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			ELSE IF @BatchMode = 1  " + System.Environment.NewLine +
                "   			BEGIN  " + System.Environment.NewLine +
                "   				INSERT INTO tblStock (  " + System.Environment.NewLine +
                "   					StockID  " + System.Environment.NewLine +
                "   					,TenantID  " + System.Environment.NewLine +
                "   					,CCID  " + System.Environment.NewLine +
                "   					,BatchCode  " + System.Environment.NewLine +
                "   					,BatchUnique  " + System.Environment.NewLine +
                "   					,BatchID  " + System.Environment.NewLine +
                "   					,MRP  " + System.Environment.NewLine +
                "   					,ExpiryDate  " + System.Environment.NewLine +
                "   					,CostRateInc  " + System.Environment.NewLine +
                "   					,CostRateExcl  " + System.Environment.NewLine +
                "   					,PRateExcl  " + System.Environment.NewLine +
                "   					,PrateInc  " + System.Environment.NewLine +
                "   					,TaxPer  " + System.Environment.NewLine +
                "   					,SRate1  " + System.Environment.NewLine +
                "   					,SRate2  " + System.Environment.NewLine +
                "   					,SRate3  " + System.Environment.NewLine +
                "   					,SRate4  " + System.Environment.NewLine +
                "   					,SRate5  " + System.Environment.NewLine +
                "   					,QOH  " + System.Environment.NewLine +
                "   					,LastInvDate  " + System.Environment.NewLine +
                "   					,LastInvNo  " + System.Environment.NewLine +
                "   					,LastSupplierID  " + System.Environment.NewLine +
                "   					,ItemID  " + System.Environment.NewLine +
                "   					,PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				VALUES (  " + System.Environment.NewLine +
                "   					@StockID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@BatchID  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@ExpiryDate  " + System.Environment.NewLine +
                "   					,@CostRateInc  " + System.Environment.NewLine +
                "   					,@CostRateExcl  " + System.Environment.NewLine +
                "   					,@PRateExcl  " + System.Environment.NewLine +
                "   					,@PrateInc  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@SRate1  " + System.Environment.NewLine +
                "   					,@SRate2  " + System.Environment.NewLine +
                "   					,@SRate3  " + System.Environment.NewLine +
                "   					,@SRate4  " + System.Environment.NewLine +
                "   					,@SRate5  " + System.Environment.NewLine +
                "   					,ABS(@QOH)  " + System.Environment.NewLine +
                "   					,@LastInvDate  " + System.Environment.NewLine +
                "   					,@LastInvNo  " + System.Environment.NewLine +
                "   					,@LastSupplierID  " + System.Environment.NewLine +
                "   					,@ItemID  " + System.Environment.NewLine +
                "   					,@PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				SET @RetResult = 1;  " + System.Environment.NewLine +
                "   				SET @TransType = 'S';  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			ELSE IF @BatchMode = 2  " + System.Environment.NewLine +
                "   				AND @BatchCode <> ''  " + System.Environment.NewLine +
                "   			BEGIN /*Auto*/  " + System.Environment.NewLine +
                "   				INSERT INTO tblStock (  " + System.Environment.NewLine +
                "   					StockID  " + System.Environment.NewLine +
                "   					,TenantID  " + System.Environment.NewLine +
                "   					,CCID  " + System.Environment.NewLine +
                "   					,BatchCode  " + System.Environment.NewLine +
                "   					,BatchUnique  " + System.Environment.NewLine +
                "   					,BatchID  " + System.Environment.NewLine +
                "   					,MRP  " + System.Environment.NewLine +
                "   					,ExpiryDate  " + System.Environment.NewLine +
                "   					,CostRateInc  " + System.Environment.NewLine +
                "   					,CostRateExcl  " + System.Environment.NewLine +
                "   					,PRateExcl  " + System.Environment.NewLine +
                "   					,PrateInc  " + System.Environment.NewLine +
                "   					,TaxPer  " + System.Environment.NewLine +
                "   					,SRate1  " + System.Environment.NewLine +
                "   					,SRate2  " + System.Environment.NewLine +
                "   					,SRate3  " + System.Environment.NewLine +
                "   					,SRate4  " + System.Environment.NewLine +
                "   					,SRate5  " + System.Environment.NewLine +
                "   					,QOH  " + System.Environment.NewLine +
                "   					,LastInvDate  " + System.Environment.NewLine +
                "   					,LastInvNo  " + System.Environment.NewLine +
                "   					,LastSupplierID  " + System.Environment.NewLine +
                "   					,ItemID  " + System.Environment.NewLine +
                "   					,PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				VALUES (  " + System.Environment.NewLine +
                "   					@StockID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@BatchID  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@ExpiryDate  " + System.Environment.NewLine +
                "   					,@CostRateInc  " + System.Environment.NewLine +
                "   					,@CostRateExcl  " + System.Environment.NewLine +
                "   					,@PRateExcl  " + System.Environment.NewLine +
                "   					,@PrateInc  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@SRate1  " + System.Environment.NewLine +
                "   					,@SRate2  " + System.Environment.NewLine +
                "   					,@SRate3  " + System.Environment.NewLine +
                "   					,@SRate4  " + System.Environment.NewLine +
                "   					,@SRate5  " + System.Environment.NewLine +
                "   					,ABS(@QOH)  " + System.Environment.NewLine +
                "   					,@LastInvDate  " + System.Environment.NewLine +
                "   					,@LastInvNo  " + System.Environment.NewLine +
                "   					,@LastSupplierID  " + System.Environment.NewLine +
                "   					,@ItemID  " + System.Environment.NewLine +
                "   					,@PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				SET @RetResult = 1;  " + System.Environment.NewLine +
                "   				SET @TransType = 'S';  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			ELSE IF @BatchMode = 3  " + System.Environment.NewLine +
                "   			BEGIN  " + System.Environment.NewLine +
                "   				INSERT INTO tblStock (  " + System.Environment.NewLine +
                "   					StockID  " + System.Environment.NewLine +
                "   					,TenantID  " + System.Environment.NewLine +
                "   					,CCID  " + System.Environment.NewLine +
                "   					,BatchCode  " + System.Environment.NewLine +
                "   					,BatchUnique  " + System.Environment.NewLine +
                "   					,BatchID  " + System.Environment.NewLine +
                "   					,MRP  " + System.Environment.NewLine +
                "   					,ExpiryDate  " + System.Environment.NewLine +
                "   					,CostRateInc  " + System.Environment.NewLine +
                "   					,CostRateExcl  " + System.Environment.NewLine +
                "   					,PRateExcl  " + System.Environment.NewLine +
                "   					,PrateInc  " + System.Environment.NewLine +
                "   					,TaxPer  " + System.Environment.NewLine +
                "   					,SRate1  " + System.Environment.NewLine +
                "   					,SRate2  " + System.Environment.NewLine +
                "   					,SRate3  " + System.Environment.NewLine +
                "   					,SRate4  " + System.Environment.NewLine +
                "   					,SRate5  " + System.Environment.NewLine +
                "   					,QOH  " + System.Environment.NewLine +
                "   					,LastInvDate  " + System.Environment.NewLine +
                "   					,LastInvNo  " + System.Environment.NewLine +
                "   					,LastSupplierID  " + System.Environment.NewLine +
                "   					,ItemID  " + System.Environment.NewLine +
                "   					,PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				VALUES (  " + System.Environment.NewLine +
                "   					@StockID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@BatchID  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@ExpiryDate  " + System.Environment.NewLine +
                "   					,@CostRateInc  " + System.Environment.NewLine +
                "   					,@CostRateExcl  " + System.Environment.NewLine +
                "   					,@PRateExcl  " + System.Environment.NewLine +
                "   					,@PrateInc  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@SRate1  " + System.Environment.NewLine +
                "   					,@SRate2  " + System.Environment.NewLine +
                "   					,@SRate3  " + System.Environment.NewLine +
                "   					,@SRate4  " + System.Environment.NewLine +
                "   					,@SRate5  " + System.Environment.NewLine +
                "   					,ABS(@QOH)  " + System.Environment.NewLine +
                "   					,@LastInvDate  " + System.Environment.NewLine +
                "   					,@LastInvNo  " + System.Environment.NewLine +
                "   					,@LastSupplierID  " + System.Environment.NewLine +
                "   					,@ItemID  " + System.Environment.NewLine +
                "   					,@PRate  " + System.Environment.NewLine +
                "   					)  " + System.Environment.NewLine +
                "   				SET @RetResult = 1;  " + System.Environment.NewLine +
                "   				SET @TransType = 'S';  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		IF @Action = 1  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			UPDATE tblStock  " + System.Environment.NewLine +
                "   			SET BatchID = @BatchID  " + System.Environment.NewLine +
                "   				,MRP = @MRP  " + System.Environment.NewLine +
                "   				,ExpiryDate = @ExpiryDate  " + System.Environment.NewLine +
                "   				,CostRateInc = @CostRateInc  " + System.Environment.NewLine +
                "   				,CostRateExcl = @CostRateExcl  " + System.Environment.NewLine +
                "   				,PRateExcl = @PRateExcl  " + System.Environment.NewLine +
                "   				,PrateInc = @PrateInc  " + System.Environment.NewLine +
                "   				,TaxPer = @TaxPer  " + System.Environment.NewLine +
                "   				,SRate1 = @SRate1  " + System.Environment.NewLine +
                "   				,SRate2 = @SRate2  " + System.Environment.NewLine +
                "   				,SRate3 = @SRate3  " + System.Environment.NewLine +
                "   				,SRate4 = @SRate4  " + System.Environment.NewLine +
                "   				,SRate5 = @SRate5  " + System.Environment.NewLine +
                "   				,QOH = QOH + @QOH  " + System.Environment.NewLine +
                "   				,LastInvDate = @LastInvDate  " + System.Environment.NewLine +
                "   				,LastInvNo = @LastInvNo  " + System.Environment.NewLine +
                "   				,LastSupplierID = @LastSupplierID  " + System.Environment.NewLine +
                "   				,PRate = @PRate  " + System.Environment.NewLine +
                "   			WHERE ItemID = @ItemID  " + System.Environment.NewLine +
                "   				AND CCID = @CCID  " + System.Environment.NewLine +
                "   				AND BatchCode = @BatchCode  " + System.Environment.NewLine +
                "   				AND BatchUnique = @BatchUnique  " + System.Environment.NewLine +
                "   				AND TenantID = @TenantID  " + System.Environment.NewLine +
                "   			SET @RetResult = 1;  " + System.Environment.NewLine +
                "   			SET @TransType = 'E';  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		IF @Action = 2  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			UPDATE tblStock  " + System.Environment.NewLine +
                "   			SET CCID = @CCID  " + System.Environment.NewLine +
                "   				,BatchCode = @BatchCode  " + System.Environment.NewLine +
                "   				,BatchUnique = @BatchUnique  " + System.Environment.NewLine +
                "   				,BatchID = @BatchID  " + System.Environment.NewLine +
                "   				,MRP = @MRP  " + System.Environment.NewLine +
                "   				,ExpiryDate = @ExpiryDate  " + System.Environment.NewLine +
                "   				,CostRateInc = @CostRateInc  " + System.Environment.NewLine +
                "   				,CostRateExcl = @CostRateExcl  " + System.Environment.NewLine +
                "   				,PRateExcl = @PRateExcl  " + System.Environment.NewLine +
                "   				,PrateInc = @PrateInc  " + System.Environment.NewLine +
                "   				,TaxPer = @TaxPer  " + System.Environment.NewLine +
                "   				,SRate1 = @SRate1  " + System.Environment.NewLine +
                "   				,SRate2 = @SRate2  " + System.Environment.NewLine +
                "   				,SRate3 = @SRate3  " + System.Environment.NewLine +
                "   				,SRate4 = @SRate4  " + System.Environment.NewLine +
                "   				,SRate5 = @SRate5  " + System.Environment.NewLine +
                "   				,QOH = QOH + @QOH  " + System.Environment.NewLine +
                "   				,LastInvDate = @LastInvDate  " + System.Environment.NewLine +
                "   				,LastInvNo = @LastInvNo  " + System.Environment.NewLine +
                "   				,LastSupplierID = @LastSupplierID  " + System.Environment.NewLine +
                "   				,PRate = @PRate  " + System.Environment.NewLine +
                "   			WHERE ItemID = @ItemID  " + System.Environment.NewLine +
                "   				AND CCID = @CCID  " + System.Environment.NewLine +
                "   				AND BatchCode = @BatchCode  " + System.Environment.NewLine +
                "   				AND BatchUnique = @BatchUnique  " + System.Environment.NewLine +
                "   				AND TenantID = @TenantID  " + System.Environment.NewLine +
                "   			SET @RetResult = 0;  " + System.Environment.NewLine +
                "   			SET @TransType = 'D';  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		COMMIT TRANSACTION;  " + System.Environment.NewLine +
                "   		SELECT @RetResult AS SqlSpResult  " + System.Environment.NewLine +
                "   			,@StockID AS TransID  " + System.Environment.NewLine +
                "   			,@TransType AS TransactType  " + System.Environment.NewLine +
                "   	END TRY  " + System.Environment.NewLine +
                "   	BEGIN CATCH  " + System.Environment.NewLine +
                "   		ROLLBACK;  " + System.Environment.NewLine +
                "   		SELECT - 1 AS SqlSpResult  " + System.Environment.NewLine +
                "   			,ERROR_NUMBER() AS ErrorNumber  " + System.Environment.NewLine +
                "   			,ERROR_STATE() AS ErrorState  " + System.Environment.NewLine +
                "   			,ERROR_SEVERITY() AS ErrorSeverity  " + System.Environment.NewLine +
                "   			,ERROR_PROCEDURE() AS ErrorProcedure  " + System.Environment.NewLine +
                "   			,ERROR_LINE() AS ErrorLine  " + System.Environment.NewLine +
                "   			,ERROR_MESSAGE() AS ErrorMessage;  " + System.Environment.NewLine +
                "   	END CATCH;  " + System.Environment.NewLine +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetPurchaseMaster') " +
                    "DROP PROCEDURE UspGetPurchaseMaster ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspGetPurchaseMaster] (  " + System.Environment.NewLine +
                "   	@InvId NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@TenantID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@VchTypeID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@blnPrevNext BIT = 0  " + System.Environment.NewLine +
                "   	)  " + System.Environment.NewLine +
                "   AS  " + System.Environment.NewLine +
                "   BEGIN  " + System.Environment.NewLine +
                "   	DECLARE @PrevVoucherNo INT  " + System.Environment.NewLine +
                "   	DECLARE @NextVoucherNo INT  " + System.Environment.NewLine +
                "   	DECLARE @InvId_Org INT  " + System.Environment.NewLine +
                "   	IF @InvId <> 0  " + System.Environment.NewLine +
                "   	BEGIN  " + System.Environment.NewLine +
                "   		IF @blnPrevNext = 0  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			SELECT party  " + System.Environment.NewLine +
                "   				,InvId  " + System.Environment.NewLine +
                "   				,InvNo  " + System.Environment.NewLine +
                "   				,AutoNum  " + System.Environment.NewLine +
                "   				,Prefix  " + System.Environment.NewLine +
                "   				,convert(VARCHAR(10), InvDate, 105) AS InvDate  " + System.Environment.NewLine +
                "   				,convert(VARCHAR(10), EffectiveDate, 105) AS EffectiveDate  " + System.Environment.NewLine +
                "   				,RefNo  " + System.Environment.NewLine +
                "   				,ReferenceAutoNO  " + System.Environment.NewLine +
                "   				,MOP  " + System.Environment.NewLine +
                "   				,TaxModeID  " + System.Environment.NewLine +
                "   				,CCID  " + System.Environment.NewLine +
                "   				,SalesManID  " + System.Environment.NewLine +
                "   				,AgentID  " + System.Environment.NewLine +
                "   				,MobileNo  " + System.Environment.NewLine +
                "   				,StateID  " + System.Environment.NewLine +
                "   				,GSTType  " + System.Environment.NewLine +
                "   				,PartyAddress  " + System.Environment.NewLine +
                "   				,GrossAmt  " + System.Environment.NewLine +
                "   				,ItemDiscountTotal  " + System.Environment.NewLine +
                "   				,DiscPer  " + System.Environment.NewLine +
                "   				,Discount  " + System.Environment.NewLine +
                "   				,Taxable  " + System.Environment.NewLine +
                "   				,NonTaxable  " + System.Environment.NewLine +
                "   				,TaxAmt  " + System.Environment.NewLine +
                "   				,OtherExpense  " + System.Environment.NewLine +
                "   				,NetAmount  " + System.Environment.NewLine +
                "   				,CashDiscount  " + System.Environment.NewLine +
                "   				,RoundOff  " + System.Environment.NewLine +
                "   				,UserNarration  " + System.Environment.NewLine +
                "   				,BillAmt  " + System.Environment.NewLine +
                "   				,PartyGSTIN  " + System.Environment.NewLine +
                "   				,Isnull(CashDisPer, 0) AS CashDisPer  " + System.Environment.NewLine +
                "   				,Isnull(CostFactor, 0) AS CostFactor  " + System.Environment.NewLine +
                "   				,LedgerId  " + System.Environment.NewLine +
                "   				,Cancelled  " + System.Environment.NewLine +
                "   				,JsonData  " + System.Environment.NewLine +
                "   			FROM tblPurchase  " + System.Environment.NewLine +
                "   			WHERE InvId = @InvId  " + System.Environment.NewLine +
                "   				AND TenantID = @TenantID  " + System.Environment.NewLine +
                "   				AND VchTypeID = @VchTypeID  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		ELSE  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			SELECT @InvId_Org = InvId  " + System.Environment.NewLine +
                "   			FROM tblPurchase  " + System.Environment.NewLine +
                "   			WHERE InvNo = @InvId  " + System.Environment.NewLine +
                "   				AND TenantID = @TenantID  " + System.Environment.NewLine +
                "   				AND VchTypeID = @VchTypeID  " + System.Environment.NewLine +
                "   			SELECT TOP 1 @PrevVoucherNo = InvId  " + System.Environment.NewLine +
                "   			FROM tblPurchase  " + System.Environment.NewLine +
                "   			WHERE InvId < @InvId_Org  " + System.Environment.NewLine +
                "   				AND VchTypeID = @VchTypeID  " + System.Environment.NewLine +
                "   			ORDER BY InvId DESC  " + System.Environment.NewLine +
                "   			SELECT TOP 1 @NextVoucherNo = InvId  " + System.Environment.NewLine +
                "   			FROM tblPurchase  " + System.Environment.NewLine +
                "   			WHERE InvId > @InvId_Org  " + System.Environment.NewLine +
                "   				AND VchTypeID = @VchTypeID  " + System.Environment.NewLine +
                "   			ORDER BY InvId ASC  " + System.Environment.NewLine +
                "   			SELECT ISNULL(@PrevVoucherNo, 0) AS PrevVoucherNo  " + System.Environment.NewLine +
                "   				,ISNULL(@NextVoucherNo, 0) AS NextVoucherNo  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   	END  " + System.Environment.NewLine +
                "   	ELSE  " + System.Environment.NewLine +
                "   	BEGIN  " + System.Environment.NewLine +
                "   		SELECT InvId  " + System.Environment.NewLine +
                "   			,InvNo AS [Invoice No]  " + System.Environment.NewLine +
                "   			,CONVERT(VARCHAR(12), InvDate) AS [Invoice Date]  " + System.Environment.NewLine +
                "   			,ISNULL(RefNo, '') + CONVERT(VARCHAR, ReferenceAutoNO) AS [Reference No]  " + System.Environment.NewLine +
                "   			,MOP  " + System.Environment.NewLine +
                "   			,Party AS [Supplier]  " + System.Environment.NewLine +
                "   			,MobileNo AS [Supplier Contact]  " + System.Environment.NewLine +
                "   			--,RoundOff AS [RoundOff]  " + System.Environment.NewLine +
                "   			,BillAmt AS [Bill Amount]  " + System.Environment.NewLine +
                "   			,(  " + System.Environment.NewLine +
                "   				CASE   " + System.Environment.NewLine +
                "   					WHEN ISNULL(Cancelled, 0) = 0  " + System.Environment.NewLine +
                "   						THEN 'Active'  " + System.Environment.NewLine +
                "   					ELSE 'Cancelled'  " + System.Environment.NewLine +
                "   					END  " + System.Environment.NewLine +
                "   				) AS [Bill Status]  " + System.Environment.NewLine +
                "   		FROM tblPurchase  " + System.Environment.NewLine +
                "   		WHERE TenantID = @TenantID  " + System.Environment.NewLine +
                "   			AND VchTypeID = @VchTypeID  " + System.Environment.NewLine +
                "   		ORDER BY InvID ASC  " + System.Environment.NewLine +
                "   	END  " + System.Environment.NewLine +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspPurchaseItemInsert') " +
                    "DROP PROCEDURE UspPurchaseItemInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspPurchaseItemInsert] (  " + System.Environment.NewLine +
                "   	@InvID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@ItemId NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Qty FLOAT  " + System.Environment.NewLine +
                "   	,@Rate FLOAT  " + System.Environment.NewLine +
                "   	,@UnitId NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Batch VARCHAR(50)  " + System.Environment.NewLine +
                "   	,@TaxPer FLOAT  " + System.Environment.NewLine +
                "   	,@TaxAmount FLOAT  " + System.Environment.NewLine +
                "   	,@Discount FLOAT  " + System.Environment.NewLine +
                "   	,@MRP FLOAT  " + System.Environment.NewLine +
                "   	,@SlNo NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Prate FLOAT  " + System.Environment.NewLine +
                "   	,@Free FLOAT  " + System.Environment.NewLine +
                "   	,@SerialNos VARCHAR(5000)  " + System.Environment.NewLine +
                "   	,@ItemDiscount FLOAT  " + System.Environment.NewLine +
                "   	,@BatchCode VARCHAR(50)  " + System.Environment.NewLine +
                "   	,@iCessOnTax FLOAT  " + System.Environment.NewLine +
                "   	,@blnCessOnTax NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Expiry DATETIME  " + System.Environment.NewLine +
                "   	,@ItemDiscountPer FLOAT  " + System.Environment.NewLine +
                "   	,@RateInclusive NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@ITaxableAmount FLOAT  " + System.Environment.NewLine +
                "   	,@INetAmount FLOAT  " + System.Environment.NewLine +
                "   	,@CGSTTaxPer FLOAT  " + System.Environment.NewLine +
                "   	,@CGSTTaxAmt FLOAT  " + System.Environment.NewLine +
                "   	,@SGSTTaxPer FLOAT  " + System.Environment.NewLine +
                "   	,@SGSTTaxAmt FLOAT  " + System.Environment.NewLine +
                "   	,@IGSTTaxPer FLOAT  " + System.Environment.NewLine +
                "   	,@IGSTTaxAmt FLOAT  " + System.Environment.NewLine +
                "   	,@iRateDiscPer FLOAT  " + System.Environment.NewLine +
                "   	,@iRateDiscount FLOAT  " + System.Environment.NewLine +
                "   	,@BatchUnique VARCHAR(150)  " + System.Environment.NewLine +
                "   	,@blnQtyIN NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@CRate FLOAT  " + System.Environment.NewLine +
                "   	,@Unit VARCHAR(50)  " + System.Environment.NewLine +
                "   	,@ItemStockID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@IcessPercent FLOAT  " + System.Environment.NewLine +
                "   	,@IcessAmt FLOAT  " + System.Environment.NewLine +
                "   	,@IQtyCompCessPer FLOAT  " + System.Environment.NewLine +
                "   	,@IQtyCompCessAmt FLOAT  " + System.Environment.NewLine +
                "   	,@StockMRP FLOAT  " + System.Environment.NewLine +
                "   	,@BaseCRate FLOAT  " + System.Environment.NewLine +
                "   	,@InonTaxableAmount FLOAT  " + System.Environment.NewLine +
                "   	,@IAgentCommPercent FLOAT  " + System.Environment.NewLine +
                "   	,@BlnDelete NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Id NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@StrOfferDetails VARCHAR(100)  " + System.Environment.NewLine +
                "   	,@BlnOfferItem FLOAT  " + System.Environment.NewLine +
                "   	,@BalQty FLOAT  " + System.Environment.NewLine +
                "   	,@GrossAmount FLOAT  " + System.Environment.NewLine +
                "   	,@iFloodCessPer FLOAT  " + System.Environment.NewLine +
                "   	,@iFloodCessAmt FLOAT  " + System.Environment.NewLine +
                "   	,@Srate1 FLOAT  " + System.Environment.NewLine +
                "   	,@Srate2 FLOAT  " + System.Environment.NewLine +
                "   	,@Srate3 FLOAT  " + System.Environment.NewLine +
                "   	,@Srate4 FLOAT  " + System.Environment.NewLine +
                "   	,@Srate5 FLOAT  " + System.Environment.NewLine +
                "   	,@Costrate FLOAT  " + System.Environment.NewLine +
                "   	,@CostValue FLOAT  " + System.Environment.NewLine +
                "   	,@Profit FLOAT  " + System.Environment.NewLine +
                "   	,@ProfitPer FLOAT  " + System.Environment.NewLine +
                "   	,@DiscMode NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@Srate1Per FLOAT  " + System.Environment.NewLine +
                "   	,@Srate2Per FLOAT  " + System.Environment.NewLine +
                "   	,@Srate3Per FLOAT  " + System.Environment.NewLine +
                "   	,@Srate4Per FLOAT  " + System.Environment.NewLine +
                "   	,@Srate5Per FLOAT  " + System.Environment.NewLine +
                "   	,@Action INT = 0  " + System.Environment.NewLine +
                "   	)  " + System.Environment.NewLine +
                "   AS  " + System.Environment.NewLine +
                "   BEGIN  " + System.Environment.NewLine +
                "   	DECLARE @RetResult INT  " + System.Environment.NewLine +
                "   	DECLARE @RetID INT  " + System.Environment.NewLine +
                "   	DECLARE @VchType VARCHAR(50)  " + System.Environment.NewLine +
                "   	DECLARE @VchTypeID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	DECLARE @BatchMode VARCHAR(50)  " + System.Environment.NewLine +
                "   	DECLARE @VchDate DATETIME  " + System.Environment.NewLine +
                "   	DECLARE @CCID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	DECLARE @TenantID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	DECLARE @BarCode_out VARCHAR(50)  " + System.Environment.NewLine +
                "   	DECLARE @VchParentID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	DECLARE @FreeQty NUMERIC(18, 5)  " + System.Environment.NewLine +
                "   	BEGIN TRY  " + System.Environment.NewLine +
                "   		BEGIN TRANSACTION;  " + System.Environment.NewLine +
                "   		SET @FreeQty = @Qty + @Free  " + System.Environment.NewLine +
                "   		SELECT @VchType = VchType  " + System.Environment.NewLine +
                "   			,@VchTypeID = VchTypeID  " + System.Environment.NewLine +
                "   			,@VchDate = InvDate  " + System.Environment.NewLine +
                "   			,@CCID = CCID  " + System.Environment.NewLine +
                "   			,@TenantID = TenantID  " + System.Environment.NewLine +
                "   		FROM tblPurchase  " + System.Environment.NewLine +
                "   		WHERE InvId = @InvID  " + System.Environment.NewLine +
                "   		SELECT @BatchMode = BatchMode  " + System.Environment.NewLine +
                "   		FROM tblItemMaster  " + System.Environment.NewLine +
                "   		WHERE ItemID = @ItemId  " + System.Environment.NewLine +
                "   		SELECT @VchParentID = ParentID  " + System.Environment.NewLine +
                "   		FROM tblVchType  " + System.Environment.NewLine +
                "   		WHERE VchTypeID = @VchTypeID  " + System.Environment.NewLine +
                "   		IF @Action = 0  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			IF @VchParentID = 2  " + System.Environment.NewLine +
                "   			BEGIN  " + System.Environment.NewLine +
                "   				EXEC UspTransStockUpdate @ItemId  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@FreeQty  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@CRate  " + System.Environment.NewLine +
                "   					,@CRate  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@Srate1  " + System.Environment.NewLine +
                "   					,@Srate2  " + System.Environment.NewLine +
                "   					,@Srate3  " + System.Environment.NewLine +
                "   					,@Srate4  " + System.Environment.NewLine +
                "   					,@Srate5  " + System.Environment.NewLine +
                "   					,@BatchMode  " + System.Environment.NewLine +
                "   					,@VchType  " + System.Environment.NewLine +
                "   					,@VchDate  " + System.Environment.NewLine +
                "   					,@Expiry  " + System.Environment.NewLine +
                "   					,'STOCKADD'  " + System.Environment.NewLine +
                "   					,@InvID  " + System.Environment.NewLine +
                "   					,@VchTypeID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@BarCode_out OUTPUT  " + System.Environment.NewLine +
                "   				IF CHARINDEX('@', @BarCode_out) > 0  " + System.Environment.NewLine +
                "   				BEGIN  " + System.Environment.NewLine +
                "   					SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  " + System.Environment.NewLine +
                "   				END  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			ELSE IF @VchParentID = 4  " + System.Environment.NewLine +
                "   			BEGIN  " + System.Environment.NewLine +
                "   				EXEC UspTransStockUpdate @ItemId  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@FreeQty  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@CRate  " + System.Environment.NewLine +
                "   					,@CRate  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@Srate1  " + System.Environment.NewLine +
                "   					,@Srate2  " + System.Environment.NewLine +
                "   					,@Srate3  " + System.Environment.NewLine +
                "   					,@Srate4  " + System.Environment.NewLine +
                "   					,@Srate5  " + System.Environment.NewLine +
                "   					,@BatchMode  " + System.Environment.NewLine +
                "   					,@VchType  " + System.Environment.NewLine +
                "   					,@VchDate  " + System.Environment.NewLine +
                "   					,@Expiry  " + System.Environment.NewLine +
                "   					,'STOCKLESS'  " + System.Environment.NewLine +
                "   					,@InvID  " + System.Environment.NewLine +
                "   					,@VchTypeID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@BarCode_out OUTPUT  " + System.Environment.NewLine +
                "   				IF CHARINDEX('@', @BarCode_out) > 0  " + System.Environment.NewLine +
                "   				BEGIN  " + System.Environment.NewLine +
                "   					SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  " + System.Environment.NewLine +
                "   				END  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			ELSE IF @VchParentID = 6  " + System.Environment.NewLine +
                "   			BEGIN  " + System.Environment.NewLine +
                "   				EXEC UspTransStockUpdate @ItemId  " + System.Environment.NewLine +
                "   					,@BatchCode  " + System.Environment.NewLine +
                "   					,@BatchUnique  " + System.Environment.NewLine +
                "   					,@FreeQty  " + System.Environment.NewLine +
                "   					,@MRP  " + System.Environment.NewLine +
                "   					,@CRate  " + System.Environment.NewLine +
                "   					,@CRate  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@TaxPer  " + System.Environment.NewLine +
                "   					,@Srate1  " + System.Environment.NewLine +
                "   					,@Srate2  " + System.Environment.NewLine +
                "   					,@Srate3  " + System.Environment.NewLine +
                "   					,@Srate4  " + System.Environment.NewLine +
                "   					,@Srate5  " + System.Environment.NewLine +
                "   					,@BatchMode  " + System.Environment.NewLine +
                "   					,@VchType  " + System.Environment.NewLine +
                "   					,@VchDate  " + System.Environment.NewLine +
                "   					,@Expiry  " + System.Environment.NewLine +
                "   					,'STOCKADD'  " + System.Environment.NewLine +
                "   					,@InvID  " + System.Environment.NewLine +
                "   					,@VchTypeID  " + System.Environment.NewLine +
                "   					,@CCID  " + System.Environment.NewLine +
                "   					,@TenantID  " + System.Environment.NewLine +
                "   					,@Prate  " + System.Environment.NewLine +
                "   					,@BarCode_out OUTPUT  " + System.Environment.NewLine +
                "   				IF CHARINDEX('@', @BarCode_out) > 0  " + System.Environment.NewLine +
                "   				BEGIN  " + System.Environment.NewLine +
                "   					SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  " + System.Environment.NewLine +
                "   				END  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			ELSE  " + System.Environment.NewLine +
                "   			BEGIN  " + System.Environment.NewLine +
                "   				SET @BatchCode = @BarCode_out  " + System.Environment.NewLine +
                "   			END  " + System.Environment.NewLine +
                "   			INSERT INTO tblPurchaseItem (  " + System.Environment.NewLine +
                "   				InvID  " + System.Environment.NewLine +
                "   				,ItemId  " + System.Environment.NewLine +
                "   				,Qty  " + System.Environment.NewLine +
                "   				,Rate  " + System.Environment.NewLine +
                "   				,UnitId  " + System.Environment.NewLine +
                "   				,Batch  " + System.Environment.NewLine +
                "   				,TaxPer  " + System.Environment.NewLine +
                "   				,TaxAmount  " + System.Environment.NewLine +
                "   				,Discount  " + System.Environment.NewLine +
                "   				,MRP  " + System.Environment.NewLine +
                "   				,SlNo  " + System.Environment.NewLine +
                "   				,Prate  " + System.Environment.NewLine +
                "   				,Free  " + System.Environment.NewLine +
                "   				,SerialNos  " + System.Environment.NewLine +
                "   				,ItemDiscount  " + System.Environment.NewLine +
                "   				,BatchCode  " + System.Environment.NewLine +
                "   				,iCessOnTax  " + System.Environment.NewLine +
                "   				,blnCessOnTax  " + System.Environment.NewLine +
                "   				,Expiry  " + System.Environment.NewLine +
                "   				,ItemDiscountPer  " + System.Environment.NewLine +
                "   				,RateInclusive  " + System.Environment.NewLine +
                "   				,ITaxableAmount  " + System.Environment.NewLine +
                "   				,INetAmount  " + System.Environment.NewLine +
                "   				,CGSTTaxPer  " + System.Environment.NewLine +
                "   				,CGSTTaxAmt  " + System.Environment.NewLine +
                "   				,SGSTTaxPer  " + System.Environment.NewLine +
                "   				,SGSTTaxAmt  " + System.Environment.NewLine +
                "   				,IGSTTaxPer  " + System.Environment.NewLine +
                "   				,IGSTTaxAmt  " + System.Environment.NewLine +
                "   				,iRateDiscPer  " + System.Environment.NewLine +
                "   				,iRateDiscount  " + System.Environment.NewLine +
                "   				,BatchUnique  " + System.Environment.NewLine +
                "   				,blnQtyIN  " + System.Environment.NewLine +
                "   				,CRate  " + System.Environment.NewLine +
                "   				,Unit  " + System.Environment.NewLine +
                "   				,ItemStockID  " + System.Environment.NewLine +
                "   				,IcessPercent  " + System.Environment.NewLine +
                "   				,IcessAmt  " + System.Environment.NewLine +
                "   				,IQtyCompCessPer  " + System.Environment.NewLine +
                "   				,IQtyCompCessAmt  " + System.Environment.NewLine +
                "   				,StockMRP  " + System.Environment.NewLine +
                "   				,BaseCRate  " + System.Environment.NewLine +
                "   				,InonTaxableAmount  " + System.Environment.NewLine +
                "   				,IAgentCommPercent  " + System.Environment.NewLine +
                "   				,BlnDelete  " + System.Environment.NewLine +
                "   				,StrOfferDetails  " + System.Environment.NewLine +
                "   				,BlnOfferItem  " + System.Environment.NewLine +
                "   				,BalQty  " + System.Environment.NewLine +
                "   				,GrossAmount  " + System.Environment.NewLine +
                "   				,iFloodCessPer  " + System.Environment.NewLine +
                "   				,iFloodCessAmt  " + System.Environment.NewLine +
                "   				,Srate1  " + System.Environment.NewLine +
                "   				,Srate2  " + System.Environment.NewLine +
                "   				,Srate3  " + System.Environment.NewLine +
                "   				,Srate4  " + System.Environment.NewLine +
                "   				,Srate5  " + System.Environment.NewLine +
                "   				,Costrate  " + System.Environment.NewLine +
                "   				,CostValue  " + System.Environment.NewLine +
                "   				,Profit  " + System.Environment.NewLine +
                "   				,ProfitPer  " + System.Environment.NewLine +
                "   				,DiscMode  " + System.Environment.NewLine +
                "   				,Srate1Per  " + System.Environment.NewLine +
                "   				,Srate2Per  " + System.Environment.NewLine +
                "   				,Srate3Per  " + System.Environment.NewLine +
                "   				,Srate4Per  " + System.Environment.NewLine +
                "   				,Srate5Per  " + System.Environment.NewLine +
                "   				)  " + System.Environment.NewLine +
                "   			VALUES (  " + System.Environment.NewLine +
                "   				@InvID  " + System.Environment.NewLine +
                "   				,@ItemId  " + System.Environment.NewLine +
                "   				,@Qty  " + System.Environment.NewLine +
                "   				,@Rate  " + System.Environment.NewLine +
                "   				,@UnitId  " + System.Environment.NewLine +
                "   				,@Batch  " + System.Environment.NewLine +
                "   				,@TaxPer  " + System.Environment.NewLine +
                "   				,@TaxAmount  " + System.Environment.NewLine +
                "   				,@Discount  " + System.Environment.NewLine +
                "   				,@MRP  " + System.Environment.NewLine +
                "   				,@SlNo  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@Free  " + System.Environment.NewLine +
                "   				,@SerialNos  " + System.Environment.NewLine +
                "   				,@ItemDiscount  " + System.Environment.NewLine +
                "   				,@BatchCode  " + System.Environment.NewLine +
                "   				,@iCessOnTax  " + System.Environment.NewLine +
                "   				,@blnCessOnTax  " + System.Environment.NewLine +
                "   				,@Expiry  " + System.Environment.NewLine +
                "   				,@ItemDiscountPer  " + System.Environment.NewLine +
                "   				,@RateInclusive  " + System.Environment.NewLine +
                "   				,@ITaxableAmount  " + System.Environment.NewLine +
                "   				,@INetAmount  " + System.Environment.NewLine +
                "   				,@CGSTTaxPer  " + System.Environment.NewLine +
                "   				,@CGSTTaxAmt  " + System.Environment.NewLine +
                "   				,@SGSTTaxPer  " + System.Environment.NewLine +
                "   				,@SGSTTaxAmt  " + System.Environment.NewLine +
                "   				,@IGSTTaxPer  " + System.Environment.NewLine +
                "   				,@IGSTTaxAmt  " + System.Environment.NewLine +
                "   				,@iRateDiscPer  " + System.Environment.NewLine +
                "   				,@iRateDiscount  " + System.Environment.NewLine +
                "   				,@BarCode_out  " + System.Environment.NewLine +
                "   				,@blnQtyIN  " + System.Environment.NewLine +
                "   				,@CRate  " + System.Environment.NewLine +
                "   				,@Unit  " + System.Environment.NewLine +
                "   				,@ItemStockID  " + System.Environment.NewLine +
                "   				,@IcessPercent  " + System.Environment.NewLine +
                "   				,@IcessAmt  " + System.Environment.NewLine +
                "   				,@IQtyCompCessPer  " + System.Environment.NewLine +
                "   				,@IQtyCompCessAmt  " + System.Environment.NewLine +
                "   				,@StockMRP  " + System.Environment.NewLine +
                "   				,@BaseCRate  " + System.Environment.NewLine +
                "   				,@InonTaxableAmount  " + System.Environment.NewLine +
                "   				,@IAgentCommPercent  " + System.Environment.NewLine +
                "   				,@BlnDelete  " + System.Environment.NewLine +
                "   				,@StrOfferDetails  " + System.Environment.NewLine +
                "   				,@BlnOfferItem  " + System.Environment.NewLine +
                "   				,@BalQty  " + System.Environment.NewLine +
                "   				,@GrossAmount  " + System.Environment.NewLine +
                "   				,@iFloodCessPer  " + System.Environment.NewLine +
                "   				,@iFloodCessAmt  " + System.Environment.NewLine +
                "   				,@Srate1  " + System.Environment.NewLine +
                "   				,@Srate2  " + System.Environment.NewLine +
                "   				,@Srate3  " + System.Environment.NewLine +
                "   				,@Srate4  " + System.Environment.NewLine +
                "   				,@Srate5  " + System.Environment.NewLine +
                "   				,@Costrate  " + System.Environment.NewLine +
                "   				,@CostValue  " + System.Environment.NewLine +
                "   				,@Profit  " + System.Environment.NewLine +
                "   				,@ProfitPer  " + System.Environment.NewLine +
                "   				,@DiscMode  " + System.Environment.NewLine +
                "   				,@Srate1Per  " + System.Environment.NewLine +
                "   				,@Srate2Per  " + System.Environment.NewLine +
                "   				,@Srate3Per  " + System.Environment.NewLine +
                "   				,@Srate4Per  " + System.Environment.NewLine +
                "   				,@Srate5Per  " + System.Environment.NewLine +
                "   				)  " + System.Environment.NewLine +
                "   			SET @RetResult = 1;  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		ELSE IF @Action = 2  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			EXEC UspTransStockUpdate @ItemId  " + System.Environment.NewLine +
                "   				,@BatchCode  " + System.Environment.NewLine +
                "   				,@BatchUnique  " + System.Environment.NewLine +
                "   				,@FreeQty  " + System.Environment.NewLine +
                "   				,@MRP  " + System.Environment.NewLine +
                "   				,@CRate  " + System.Environment.NewLine +
                "   				,@CRate  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@TaxPer  " + System.Environment.NewLine +
                "   				,@Srate1  " + System.Environment.NewLine +
                "   				,@Srate2  " + System.Environment.NewLine +
                "   				,@Srate3  " + System.Environment.NewLine +
                "   				,@Srate4  " + System.Environment.NewLine +
                "   				,@Srate5  " + System.Environment.NewLine +
                "   				,@BatchMode  " + System.Environment.NewLine +
                "   				,@VchType  " + System.Environment.NewLine +
                "   				,@VchDate  " + System.Environment.NewLine +
                "   				,@Expiry  " + System.Environment.NewLine +
                "   				,'STOCKDEL'  " + System.Environment.NewLine +
                "   				,@InvID  " + System.Environment.NewLine +
                "   				,@VchTypeID  " + System.Environment.NewLine +
                "   				,@CCID  " + System.Environment.NewLine +
                "   				,@TenantID  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@BarCode_out OUTPUT  " + System.Environment.NewLine +
                "   			DELETE  " + System.Environment.NewLine +
                "   			FROM tblPurchaseItem  " + System.Environment.NewLine +
                "   			WHERE InvID = @InvID  " + System.Environment.NewLine +
                "   			SET @RetResult = 0;  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		ELSE IF @Action = 3  " + System.Environment.NewLine +
                "   		BEGIN  " + System.Environment.NewLine +
                "   			EXEC UspTransStockUpdate @ItemId  " + System.Environment.NewLine +
                "   				,@BatchCode  " + System.Environment.NewLine +
                "   				,@BatchUnique  " + System.Environment.NewLine +
                "   				,@FreeQty  " + System.Environment.NewLine +
                "   				,@MRP  " + System.Environment.NewLine +
                "   				,@CRate  " + System.Environment.NewLine +
                "   				,@CRate  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@TaxPer  " + System.Environment.NewLine +
                "   				,@Srate1  " + System.Environment.NewLine +
                "   				,@Srate2  " + System.Environment.NewLine +
                "   				,@Srate3  " + System.Environment.NewLine +
                "   				,@Srate4  " + System.Environment.NewLine +
                "   				,@Srate5  " + System.Environment.NewLine +
                "   				,@BatchMode  " + System.Environment.NewLine +
                "   				,@VchType  " + System.Environment.NewLine +
                "   				,@VchDate  " + System.Environment.NewLine +
                "   				,@Expiry  " + System.Environment.NewLine +
                "   				,'STOCKDEL'  " + System.Environment.NewLine +
                "   				,@InvID  " + System.Environment.NewLine +
                "   				,@VchTypeID  " + System.Environment.NewLine +
                "   				,@CCID  " + System.Environment.NewLine +
                "   				,@TenantID  " + System.Environment.NewLine +
                "   				,@Prate  " + System.Environment.NewLine +
                "   				,@BarCode_out OUTPUT  " + System.Environment.NewLine +
                "   			SET @RetResult = 0;  " + System.Environment.NewLine +
                "   		END  " + System.Environment.NewLine +
                "   		COMMIT TRANSACTION;  " + System.Environment.NewLine +
                "   		SELECT @RetResult AS SqlSpResult  " + System.Environment.NewLine +
                "   			,@RetID AS PID  " + System.Environment.NewLine +
                "   	END TRY  " + System.Environment.NewLine +
                "   	BEGIN CATCH  " + System.Environment.NewLine +
                "   		ROLLBACK;  " + System.Environment.NewLine +
                "   		SELECT - 1 AS SqlSpResult  " + System.Environment.NewLine +
                "   			,@RetID AS PID  " + System.Environment.NewLine +
                "   			,ERROR_NUMBER() AS ErrorNumber  " + System.Environment.NewLine +
                "   			,ERROR_STATE() AS ErrorState  " + System.Environment.NewLine +
                "   			,ERROR_SEVERITY() AS ErrorSeverity  " + System.Environment.NewLine +
                "   			,ERROR_PROCEDURE() AS ErrorProcedure  " + System.Environment.NewLine +
                "   			,ERROR_LINE() AS ErrorLine  " + System.Environment.NewLine +
                "   			,ERROR_MESSAGE() AS ErrorMessage;  " + System.Environment.NewLine +
                "   	END CATCH;  " + System.Environment.NewLine +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspGetStockHistory') " +
                    "DROP PROCEDURE UspGetStockHistory ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "   CREATE PROCEDURE [dbo].[UspGetStockHistory] (  " + System.Environment.NewLine +
                "   	@ItemID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	,@VchTypeID NUMERIC(18, 0) = NULL  " + System.Environment.NewLine +
                "   	,@BatchUnique VARCHAR(MAX) = NULL  " + System.Environment.NewLine +
                "   	,@CostCentreID NUMERIC(18, 0) = NULL  " + System.Environment.NewLine +
                "   	,@FromDate DATETIME  " + System.Environment.NewLine +
                "   	,@ToDate DATETIME  " + System.Environment.NewLine +
                "   	,@TenantID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   	)  " + System.Environment.NewLine +
                "   AS  " + System.Environment.NewLine +
                "   BEGIN  " + System.Environment.NewLine +
                "   	DECLARE @TempTable TABLE (  " + System.Environment.NewLine +
                "   		VoucherType VARCHAR(500)  " + System.Environment.NewLine +
                "   		,InvoiceNo VARCHAR(50)  " + System.Environment.NewLine +
                "   		,VoucherDate DATETIME  " + System.Environment.NewLine +
                "   		,Batch VARCHAR(500)  " + System.Environment.NewLine +
                "   		,QtyIn NUMERIC(18, 5)  " + System.Environment.NewLine +
                "   		,QtyOut NUMERIC(18, 5)  " + System.Environment.NewLine +
                "   		,PRate NUMERIC(18, 4)  " + System.Environment.NewLine +
                "   		,SRate NUMERIC(18, 4)  " + System.Environment.NewLine +
                "   		,Vchtypeid NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   		,StockID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   		,Unit VARCHAR(500)  " + System.Environment.NewLine +
                "   		,ItemID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   		,ItemCode VARCHAR(500)  " + System.Environment.NewLine +
                "   		,CCID NUMERIC(18, 0)  " + System.Environment.NewLine +
                "   		)  " + System.Environment.NewLine +
                "   	INSERT INTO @TempTable  " + System.Environment.NewLine +
                "   	SELECT TSH.vchtype  " + System.Environment.NewLine +
                "   		,Invno  " + System.Environment.NewLine +
                "   		,VchDate  " + System.Environment.NewLine +
                "   		,TSH.batchUnique  " + System.Environment.NewLine +
                "   		,round(isnull(QTYIN, 0), 5)  " + System.Environment.NewLine +
                "   		,round(isnull(QTYOUT, 0), 5)  " + System.Environment.NewLine +
                "   		,Round(TSH.PRateExcl, 2)  " + System.Environment.NewLine +
                "   		,Round(TSH.SRate1, 2)  " + System.Environment.NewLine +
                "   		,VIA.Vchtypeid  " + System.Environment.NewLine +
                "   		,TS.StockID  " + System.Environment.NewLine +
                "   		,TU.UnitShortName  " + System.Environment.NewLine +
                "   		,TSH.ItemID  " + System.Environment.NewLine +
                "   		,ItemCode  " + System.Environment.NewLine +
                "   		,TSH.CCID  " + System.Environment.NewLine +
                "   	FROM tblStockHistory TSH  " + System.Environment.NewLine +
                "   	LEFT JOIN tblstock TS ON TSH.BatchUnique = TS.batchUnique  " + System.Environment.NewLine +
                "   	LEFT JOIN VWitemAnalysis VIA ON TSH.RefId = VIA.Invid  " + System.Environment.NewLine +
                "   	LEFT JOIN tblItemMaster IM ON TSH.ItemID = IM.ItemID  " + System.Environment.NewLine +
                "   	LEFT JOIN tblUnit TU ON IM.UNITID = TU.UnitID  " + System.Environment.NewLine +
                "   	WHERE TSH.TenantID = @TenantID  " + System.Environment.NewLine +
                "   		AND TSH.ItemID = @ItemID  " + System.Environment.NewLine +
                "   		AND convert(DATETIME, VchDate, 106) >= @FromDate  " + System.Environment.NewLine +
                "   		AND convert(DATETIME, VchDate, 106) <= @ToDate  " + System.Environment.NewLine +
                "   		AND VIA.VchType IS NOT NULL  " + System.Environment.NewLine +
                "   	IF @VchTypeID <> 0  " + System.Environment.NewLine +
                "   	BEGIN  " + System.Environment.NewLine +
                "   		DELETE  " + System.Environment.NewLine +
                "   		FROM @TempTable  " + System.Environment.NewLine +
                "   		WHERE VchTypeID <> @VchTypeID  " + System.Environment.NewLine +
                "   	END  " + System.Environment.NewLine +
                "   	IF @BatchUnique <> 0  " + System.Environment.NewLine +
                "   	BEGIN  " + System.Environment.NewLine +
                "   		DELETE  " + System.Environment.NewLine +
                "   		FROM @TempTable  " + System.Environment.NewLine +
                "   		WHERE StockID <> @BatchUnique  " + System.Environment.NewLine +
                "   	END  " + System.Environment.NewLine +
                "   	IF @CostCentreID <> 0  " + System.Environment.NewLine +
                "   	BEGIN  " + System.Environment.NewLine +
                "   		DELETE  " + System.Environment.NewLine +
                "   		FROM @TempTable  " + System.Environment.NewLine +
                "   		WHERE CCID <> @CostCentreID  " + System.Environment.NewLine +
                "   	END  " + System.Environment.NewLine +
                "   	SELECT VoucherType AS [Voucher Type]  " + System.Environment.NewLine +
                "   		,InvoiceNo AS [Invoice No]  " + System.Environment.NewLine +
                "   		,CONVERT(VARCHAR(12), FORMAT(VoucherDate, 'dd-MMM-yyyy')) AS [Voucher Date]  " + System.Environment.NewLine +
                "   		,Batch AS [Batch]  " + System.Environment.NewLine +
                "   		,QtyIn AS [Qty In]  " + System.Environment.NewLine +
                "   		,QtyOut AS [Qty Out]  " + System.Environment.NewLine +
                "   		,Unit AS [Unit]  " + System.Environment.NewLine +
                "   		,PRate AS [P.Rate]  " + System.Environment.NewLine +
                "   		,SRate AS [S.Rate]  " + System.Environment.NewLine +
                "   	FROM @TempTable  " + System.Environment.NewLine +
                "   	ORDER BY VoucherDate  " + System.Environment.NewLine +
                "   END ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "IF EXISTS(SELECT * FROM dbo.Sysobjects WHERE Xtype = 'P' AND name = 'UspStockInsert') " +
                    "DROP PROCEDURE UspStockInsert ";
            Comm.fnExecuteNonQuery(sQuery, false);

            sQuery = "      CREATE PROCEDURE [dbo].[UspStockInsert] (    " + System.Environment.NewLine +
                "      	@StockID NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	,@TenantID NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	,@CCID NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	,@BatchCode VARCHAR(100)    " + System.Environment.NewLine +
                "      	,@BatchUnique VARCHAR(50)    " + System.Environment.NewLine +
                "      	,@BatchID NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	,@MRP NUMERIC(18, 5)    " + System.Environment.NewLine +
                "      	,@ExpiryDate DATE    " + System.Environment.NewLine +
                "      	,@CostRateInc DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@CostRateExcl DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@PRateExcl DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@PrateInc DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@TaxPer DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@SRate1 DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@SRate2 DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@SRate3 DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@SRate4 DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@SRate5 DECIMAL(18, 2)    " + System.Environment.NewLine +
                "      	,@QOH DECIMAL(18, 5)      " + System.Environment.NewLine +
                "      	,@LastInvDate DATE    " + System.Environment.NewLine +
                "      	,@LastInvNo VARCHAR(50)    " + System.Environment.NewLine +
                "      	,@LastSupplierID NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	,@Action INT = 0    " + System.Environment.NewLine +
                "      	,@ItemID NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	,@BatchMode VARCHAR(100)    " + System.Environment.NewLine +
                "      	,@PRate NUMERIC(18, 5)    " + System.Environment.NewLine +
                "      	)    " + System.Environment.NewLine +
                "      AS    " + System.Environment.NewLine +
                "      BEGIN    " + System.Environment.NewLine +
                "      	DECLARE @RetResult INT    " + System.Environment.NewLine +
                "      	DECLARE @TransType CHAR(1)    " + System.Environment.NewLine +
                "      	DECLARE @blnExpiry NUMERIC(18, 0)    " + System.Environment.NewLine +
                "      	BEGIN TRY    " + System.Environment.NewLine +
                "      		BEGIN TRANSACTION;    " + System.Environment.NewLine +
                "      		IF @BatchMode = 0    " + System.Environment.NewLine +
                "      		BEGIN    " + System.Environment.NewLine +
                "      			SELECT @BatchCode = ItemCode    " + System.Environment.NewLine +
                "      				,@blnExpiry = ISNULL(blnExpiry, 0)    " + System.Environment.NewLine +
                "      			FROM tblItemMaster    " + System.Environment.NewLine +
                "      			WHERE ItemID = @ItemID    " + System.Environment.NewLine +
                "      		END    " + System.Environment.NewLine +
                "      		IF @Action = 0    " + System.Environment.NewLine +
                "      		BEGIN    " + System.Environment.NewLine +
                "      			IF @BatchMode = 0    " + System.Environment.NewLine +
                "      			BEGIN /*None*/    " + System.Environment.NewLine +
                "      				INSERT INTO tblStock (    " + System.Environment.NewLine +
                "      					StockID    " + System.Environment.NewLine +
                "      					,TenantID    " + System.Environment.NewLine +
                "      					,CCID    " + System.Environment.NewLine +
                "      					,BatchCode    " + System.Environment.NewLine +
                "      					,BatchUnique    " + System.Environment.NewLine +
                "      					,BatchID    " + System.Environment.NewLine +
                "      					,MRP    " + System.Environment.NewLine +
                "      					,ExpiryDate    " + System.Environment.NewLine +
                "      					,CostRateInc    " + System.Environment.NewLine +
                "      					,CostRateExcl    " + System.Environment.NewLine +
                "      					,PRateExcl    " + System.Environment.NewLine +
                "      					,PrateInc    " + System.Environment.NewLine +
                "      					,TaxPer    " + System.Environment.NewLine +
                "      					,SRate1    " + System.Environment.NewLine +
                "      					,SRate2    " + System.Environment.NewLine +
                "      					,SRate3    " + System.Environment.NewLine +
                "      					,SRate4    " + System.Environment.NewLine +
                "      					,SRate5    " + System.Environment.NewLine +
                "      					,QOH    " + System.Environment.NewLine +
                "      					,LastInvDate    " + System.Environment.NewLine +
                "      					,LastInvNo    " + System.Environment.NewLine +
                "      					,LastSupplierID    " + System.Environment.NewLine +
                "      					,ItemID    " + System.Environment.NewLine +
                "      					,PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				VALUES (    " + System.Environment.NewLine +
                "      					@StockID    " + System.Environment.NewLine +
                "      					,@TenantID    " + System.Environment.NewLine +
                "      					,@CCID    " + System.Environment.NewLine +
                "      					,@BatchCode    " + System.Environment.NewLine +
                "      					,@BatchUnique    " + System.Environment.NewLine +
                "      					,@BatchID    " + System.Environment.NewLine +
                "      					,@MRP    " + System.Environment.NewLine +
                "      					,@ExpiryDate    " + System.Environment.NewLine +
                "      					,@CostRateInc    " + System.Environment.NewLine +
                "      					,@CostRateExcl    " + System.Environment.NewLine +
                "      					,@PRateExcl    " + System.Environment.NewLine +
                "      					,@PrateInc    " + System.Environment.NewLine +
                "      					,@TaxPer    " + System.Environment.NewLine +
                "      					,@SRate1    " + System.Environment.NewLine +
                "      					,@SRate2    " + System.Environment.NewLine +
                "      					,@SRate3    " + System.Environment.NewLine +
                "      					,@SRate4    " + System.Environment.NewLine +
                "      					,@SRate5    " + System.Environment.NewLine +
                "      					,ABS(@QOH)    " + System.Environment.NewLine +
                "      					,@LastInvDate    " + System.Environment.NewLine +
                "      					,@LastInvNo    " + System.Environment.NewLine +
                "      					,@LastSupplierID    " + System.Environment.NewLine +
                "      					,@ItemID    " + System.Environment.NewLine +
                "      					,@PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				SET @RetResult = 1;    " + System.Environment.NewLine +
                "      				SET @TransType = 'S';    " + System.Environment.NewLine +
                "      			END    " + System.Environment.NewLine +
                "      			ELSE IF @BatchMode = 1    " + System.Environment.NewLine +
                "      			BEGIN    " + System.Environment.NewLine +
                "      				INSERT INTO tblStock (    " + System.Environment.NewLine +
                "      					StockID    " + System.Environment.NewLine +
                "      					,TenantID    " + System.Environment.NewLine +
                "      					,CCID    " + System.Environment.NewLine +
                "      					,BatchCode    " + System.Environment.NewLine +
                "      					,BatchUnique    " + System.Environment.NewLine +
                "      					,BatchID    " + System.Environment.NewLine +
                "      					,MRP    " + System.Environment.NewLine +
                "      					,ExpiryDate    " + System.Environment.NewLine +
                "      					,CostRateInc    " + System.Environment.NewLine +
                "      					,CostRateExcl    " + System.Environment.NewLine +
                "      					,PRateExcl    " + System.Environment.NewLine +
                "      					,PrateInc    " + System.Environment.NewLine +
                "      					,TaxPer    " + System.Environment.NewLine +
                "      					,SRate1    " + System.Environment.NewLine +
                "      					,SRate2    " + System.Environment.NewLine +
                "      					,SRate3    " + System.Environment.NewLine +
                "      					,SRate4    " + System.Environment.NewLine +
                "      					,SRate5    " + System.Environment.NewLine +
                "      					,QOH    " + System.Environment.NewLine +
                "      					,LastInvDate    " + System.Environment.NewLine +
                "      					,LastInvNo    " + System.Environment.NewLine +
                "      					,LastSupplierID    " + System.Environment.NewLine +
                "      					,ItemID    " + System.Environment.NewLine +
                "      					,PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				VALUES (    " + System.Environment.NewLine +
                "      					@StockID    " + System.Environment.NewLine +
                "      					,@TenantID    " + System.Environment.NewLine +
                "      					,@CCID    " + System.Environment.NewLine +
                "      					,@BatchCode    " + System.Environment.NewLine +
                "      					,@BatchUnique    " + System.Environment.NewLine +
                "      					,@BatchID    " + System.Environment.NewLine +
                "      					,@MRP    " + System.Environment.NewLine +
                "      					,@ExpiryDate    " + System.Environment.NewLine +
                "      					,@CostRateInc    " + System.Environment.NewLine +
                "      					,@CostRateExcl    " + System.Environment.NewLine +
                "      					,@PRateExcl    " + System.Environment.NewLine +
                "      					,@PrateInc    " + System.Environment.NewLine +
                "      					,@TaxPer    " + System.Environment.NewLine +
                "      					,@SRate1    " + System.Environment.NewLine +
                "      					,@SRate2    " + System.Environment.NewLine +
                "      					,@SRate3    " + System.Environment.NewLine +
                "      					,@SRate4    " + System.Environment.NewLine +
                "      					,@SRate5    " + System.Environment.NewLine +
                "      					,ABS(@QOH)    " + System.Environment.NewLine +
                "      					,@LastInvDate    " + System.Environment.NewLine +
                "      					,@LastInvNo    " + System.Environment.NewLine +
                "      					,@LastSupplierID    " + System.Environment.NewLine +
                "      					,@ItemID    " + System.Environment.NewLine +
                "      					,@PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				SET @RetResult = 1;    " + System.Environment.NewLine +
                "      				SET @TransType = 'S';    " + System.Environment.NewLine +
                "      			END    " + System.Environment.NewLine +
                "      			ELSE IF @BatchMode = 2    " + System.Environment.NewLine +
                "      				AND @BatchCode <> ''    " + System.Environment.NewLine +
                "      			BEGIN /*Auto*/    " + System.Environment.NewLine +
                "      				INSERT INTO tblStock (    " + System.Environment.NewLine +
                "      					StockID    " + System.Environment.NewLine +
                "      					,TenantID    " + System.Environment.NewLine +
                "      					,CCID    " + System.Environment.NewLine +
                "      					,BatchCode    " + System.Environment.NewLine +
                "      					,BatchUnique    " + System.Environment.NewLine +
                "      					,BatchID    " + System.Environment.NewLine +
                "      					,MRP    " + System.Environment.NewLine +
                "      					,ExpiryDate    " + System.Environment.NewLine +
                "      					,CostRateInc    " + System.Environment.NewLine +
                "      					,CostRateExcl    " + System.Environment.NewLine +
                "      					,PRateExcl    " + System.Environment.NewLine +
                "      					,PrateInc    " + System.Environment.NewLine +
                "      					,TaxPer    " + System.Environment.NewLine +
                "      					,SRate1    " + System.Environment.NewLine +
                "      					,SRate2    " + System.Environment.NewLine +
                "      					,SRate3    " + System.Environment.NewLine +
                "      					,SRate4    " + System.Environment.NewLine +
                "      					,SRate5    " + System.Environment.NewLine +
                "      					,QOH    " + System.Environment.NewLine +
                "      					,LastInvDate    " + System.Environment.NewLine +
                "      					,LastInvNo    " + System.Environment.NewLine +
                "      					,LastSupplierID    " + System.Environment.NewLine +
                "      					,ItemID    " + System.Environment.NewLine +
                "      					,PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				VALUES (    " + System.Environment.NewLine +
                "      					@StockID    " + System.Environment.NewLine +
                "      					,@TenantID    " + System.Environment.NewLine +
                "      					,@CCID    " + System.Environment.NewLine +
                "      					,@BatchCode    " + System.Environment.NewLine +
                "      					,@BatchUnique    " + System.Environment.NewLine +
                "      					,@BatchID    " + System.Environment.NewLine +
                "      					,@MRP    " + System.Environment.NewLine +
                "      					,@ExpiryDate    " + System.Environment.NewLine +
                "      					,@CostRateInc    " + System.Environment.NewLine +
                "      					,@CostRateExcl    " + System.Environment.NewLine +
                "      					,@PRateExcl    " + System.Environment.NewLine +
                "      					,@PrateInc    " + System.Environment.NewLine +
                "      					,@TaxPer    " + System.Environment.NewLine +
                "      					,@SRate1    " + System.Environment.NewLine +
                "      					,@SRate2    " + System.Environment.NewLine +
                "      					,@SRate3    " + System.Environment.NewLine +
                "      					,@SRate4    " + System.Environment.NewLine +
                "      					,@SRate5    " + System.Environment.NewLine +
                "      					,ABS(@QOH)    " + System.Environment.NewLine +
                "      					,@LastInvDate    " + System.Environment.NewLine +
                "      					,@LastInvNo    " + System.Environment.NewLine +
                "      					,@LastSupplierID    " + System.Environment.NewLine +
                "      					,@ItemID    " + System.Environment.NewLine +
                "      					,@PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				SET @RetResult = 1;    " + System.Environment.NewLine +
                "      				SET @TransType = 'S';    " + System.Environment.NewLine +
                "      			END    " + System.Environment.NewLine +
                "      			ELSE IF @BatchMode = 3    " + System.Environment.NewLine +
                "      			BEGIN    " + System.Environment.NewLine +
                "      				INSERT INTO tblStock (    " + System.Environment.NewLine +
                "      					StockID    " + System.Environment.NewLine +
                "      					,TenantID    " + System.Environment.NewLine +
                "      					,CCID    " + System.Environment.NewLine +
                "      					,BatchCode    " + System.Environment.NewLine +
                "      					,BatchUnique    " + System.Environment.NewLine +
                "      					,BatchID    " + System.Environment.NewLine +
                "      					,MRP    " + System.Environment.NewLine +
                "      					,ExpiryDate    " + System.Environment.NewLine +
                "      					,CostRateInc    " + System.Environment.NewLine +
                "      					,CostRateExcl    " + System.Environment.NewLine +
                "      					,PRateExcl    " + System.Environment.NewLine +
                "      					,PrateInc    " + System.Environment.NewLine +
                "      					,TaxPer    " + System.Environment.NewLine +
                "      					,SRate1    " + System.Environment.NewLine +
                "      					,SRate2    " + System.Environment.NewLine +
                "      					,SRate3    " + System.Environment.NewLine +
                "      					,SRate4    " + System.Environment.NewLine +
                "      					,SRate5    " + System.Environment.NewLine +
                "      					,QOH    " + System.Environment.NewLine +
                "      					,LastInvDate    " + System.Environment.NewLine +
                "      					,LastInvNo    " + System.Environment.NewLine +
                "      					,LastSupplierID    " + System.Environment.NewLine +
                "      					,ItemID    " + System.Environment.NewLine +
                "      					,PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				VALUES (    " + System.Environment.NewLine +
                "      					@StockID    " + System.Environment.NewLine +
                "      					,@TenantID    " + System.Environment.NewLine +
                "      					,@CCID    " + System.Environment.NewLine +
                "      					,@BatchCode    " + System.Environment.NewLine +
                "      					,@BatchUnique    " + System.Environment.NewLine +
                "      					,@BatchID    " + System.Environment.NewLine +
                "      					,@MRP    " + System.Environment.NewLine +
                "      					,@ExpiryDate    " + System.Environment.NewLine +
                "      					,@CostRateInc    " + System.Environment.NewLine +
                "      					,@CostRateExcl    " + System.Environment.NewLine +
                "      					,@PRateExcl    " + System.Environment.NewLine +
                "      					,@PrateInc    " + System.Environment.NewLine +
                "      					,@TaxPer    " + System.Environment.NewLine +
                "      					,@SRate1    " + System.Environment.NewLine +
                "      					,@SRate2    " + System.Environment.NewLine +
                "      					,@SRate3    " + System.Environment.NewLine +
                "      					,@SRate4    " + System.Environment.NewLine +
                "      					,@SRate5    " + System.Environment.NewLine +
                "      					,ABS(@QOH)    " + System.Environment.NewLine +
                "      					,@LastInvDate    " + System.Environment.NewLine +
                "      					,@LastInvNo    " + System.Environment.NewLine +
                "      					,@LastSupplierID    " + System.Environment.NewLine +
                "      					,@ItemID    " + System.Environment.NewLine +
                "      					,@PRate    " + System.Environment.NewLine +
                "      					)    " + System.Environment.NewLine +
                "      				SET @RetResult = 1;    " + System.Environment.NewLine +
                "      				SET @TransType = 'S';    " + System.Environment.NewLine +
                "      			END    " + System.Environment.NewLine +
                "      		END    " + System.Environment.NewLine +
                "      		IF @Action = 1    " + System.Environment.NewLine +
                "      		BEGIN    " + System.Environment.NewLine +
                "      			UPDATE tblStock    " + System.Environment.NewLine +
                "      			SET BatchID = @BatchID    " + System.Environment.NewLine +
                "      				,MRP = @MRP    " + System.Environment.NewLine +
                "      				,ExpiryDate = @ExpiryDate    " + System.Environment.NewLine +
                "      				,CostRateInc = @CostRateInc    " + System.Environment.NewLine +
                "      				,CostRateExcl = @CostRateExcl    " + System.Environment.NewLine +
                "      				,PRateExcl = @PRateExcl    " + System.Environment.NewLine +
                "      				,PrateInc = @PrateInc    " + System.Environment.NewLine +
                "      				,TaxPer = @TaxPer    " + System.Environment.NewLine +
                "      				,SRate1 = @SRate1    " + System.Environment.NewLine +
                "      				,SRate2 = @SRate2    " + System.Environment.NewLine +
                "      				,SRate3 = @SRate3    " + System.Environment.NewLine +
                "      				,SRate4 = @SRate4    " + System.Environment.NewLine +
                "      				,SRate5 = @SRate5    " + System.Environment.NewLine +
                "      				,QOH = QOH + @QOH    " + System.Environment.NewLine +
                "      				,LastInvDate = @LastInvDate    " + System.Environment.NewLine +
                "      				,LastInvNo = @LastInvNo    " + System.Environment.NewLine +
                "      				,LastSupplierID = @LastSupplierID    " + System.Environment.NewLine +
                "      				,PRate = @PRate    " + System.Environment.NewLine +
                "      			WHERE ItemID = @ItemID    " + System.Environment.NewLine +
                "      				AND CCID = @CCID    " + System.Environment.NewLine +
                "      				AND BatchCode = @BatchCode    " + System.Environment.NewLine +
                "      				AND BatchUnique = @BatchUnique    " + System.Environment.NewLine +
                "      				AND TenantID = @TenantID    " + System.Environment.NewLine +
                "      			SET @RetResult = 1;    " + System.Environment.NewLine +
                "      			SET @TransType = 'E';    " + System.Environment.NewLine +
                "      		END    " + System.Environment.NewLine +
                "      		IF @Action = 2    " + System.Environment.NewLine +
                "      		BEGIN    " + System.Environment.NewLine +
                "      			UPDATE tblStock    " + System.Environment.NewLine +
                "      			SET CCID = @CCID    " + System.Environment.NewLine +
                "      				,BatchCode = @BatchCode    " + System.Environment.NewLine +
                "      				,BatchUnique = @BatchUnique    " + System.Environment.NewLine +
                "      				,BatchID = @BatchID    " + System.Environment.NewLine +
                "      				,MRP = @MRP    " + System.Environment.NewLine +
                "      				,ExpiryDate = @ExpiryDate    " + System.Environment.NewLine +
                "      				,CostRateInc = @CostRateInc    " + System.Environment.NewLine +
                "      				,CostRateExcl = @CostRateExcl    " + System.Environment.NewLine +
                "      				,PRateExcl = @PRateExcl    " + System.Environment.NewLine +
                "      				,PrateInc = @PrateInc    " + System.Environment.NewLine +
                "      				,TaxPer = @TaxPer    " + System.Environment.NewLine +
                "      				,SRate1 = @SRate1    " + System.Environment.NewLine +
                "      				,SRate2 = @SRate2    " + System.Environment.NewLine +
                "      				,SRate3 = @SRate3    " + System.Environment.NewLine +
                "      				,SRate4 = @SRate4    " + System.Environment.NewLine +
                "      				,SRate5 = @SRate5    " + System.Environment.NewLine +
                "      				,QOH = QOH + @QOH    " + System.Environment.NewLine +
                "      				,LastInvDate = @LastInvDate    " + System.Environment.NewLine +
                "      				,LastInvNo = @LastInvNo    " + System.Environment.NewLine +
                "      				,LastSupplierID = @LastSupplierID    " + System.Environment.NewLine +
                "      				,PRate = @PRate    " + System.Environment.NewLine +
                "      			WHERE ItemID = @ItemID    " + System.Environment.NewLine +
                "      				AND CCID = @CCID    " + System.Environment.NewLine +
                "      				AND BatchCode = @BatchCode    " + System.Environment.NewLine +
                "      				AND BatchUnique = @BatchUnique    " + System.Environment.NewLine +
                "      				AND TenantID = @TenantID    " + System.Environment.NewLine +
                "      			SET @RetResult = 0;    " + System.Environment.NewLine +
                "      			SET @TransType = 'D';    " + System.Environment.NewLine +
                "      		END    " + System.Environment.NewLine +
                "      		COMMIT TRANSACTION;    " + System.Environment.NewLine +
                "      		SELECT @RetResult AS SqlSpResult    " + System.Environment.NewLine +
                "      			,@StockID AS TransID    " + System.Environment.NewLine +
                "      			,@TransType AS TransactType    " + System.Environment.NewLine +
                "      	END TRY    " + System.Environment.NewLine +
                "      	BEGIN CATCH    " + System.Environment.NewLine +
                "      		ROLLBACK;    " + System.Environment.NewLine +
                "      		SELECT - 1 AS SqlSpResult    " + System.Environment.NewLine +
                "      			,ERROR_NUMBER() AS ErrorNumber    " + System.Environment.NewLine +
                "      			,ERROR_STATE() AS ErrorState    " + System.Environment.NewLine +
                "      			,ERROR_SEVERITY() AS ErrorSeverity    " + System.Environment.NewLine +
                "      			,ERROR_PROCEDURE() AS ErrorProcedure    " + System.Environment.NewLine +
                "      			,ERROR_LINE() AS ErrorLine    " + System.Environment.NewLine +
                "      			,ERROR_MESSAGE() AS ErrorMessage;    " + System.Environment.NewLine +
                "      	END CATCH;    " + System.Environment.NewLine +
                "      END  ";
            Comm.fnExecuteNonQuery(sQuery, false);
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

        private void rdoPriceList_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tbtnAdvanced_Click(object sender, EventArgs e)
        {

        }

        private void tbtnSize_Click(object sender, EventArgs e)
        {

        }

        private void tbtnBarcode_Click(object sender, EventArgs e)
        {

        }

        private void frmSettings_Activated(object sender, EventArgs e)
        {
            try
            {
                tbtnBarcode.Enabled = false;
                tbtnAdvanced.Enabled = false;
                tbtnPLUAuto.Enabled = false;

                if (Global.gblUserName.ToUpper() == "DIGIPOS")
                {
                    tbtnBarcode.Enabled = true;

                    if (tbtnBarcode.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    {
                        tbtnAdvanced.Enabled = true;
                        tbtnPLUAuto.Enabled = true;
                    }
                    else
                    {
                        tbtnAdvanced.Enabled = false;
                        tbtnPLUAuto.Enabled = false;
                    }
                }
                else
                {
                    tbtnBarcode.Enabled = false;
                    tbtnAdvanced.Enabled = false;
                    tbtnPLUAuto.Enabled = false;
                }

                sqlControl rs = new sqlControl();
                rs.Open("Select count(itemid) as cnt from tblitemmaster");
                if (!rs.eof())
                {
                    if (rs.fields("cnt") != null)
                    {
                        if (rs.fields("cnt").ToString() != "")
                        {
                            if (Convert.ToDecimal(rs.fields("cnt").ToString()) <= 0)
                            {
                                tbtnBarcode.Enabled = true;
                                tbtnAdvanced.Enabled = true;
                                tbtnPLUAuto.Enabled = true;
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        private void tbtnBarcode_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            try
            {
                if (tbtnBarcode.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    tbtnAdvanced.Enabled = true;
                    tbtnPLUAuto.Enabled = true;
                }
                else
                {
                    tbtnAdvanced.Enabled = false;
                    tbtnPLUAuto.Enabled = false;
                }
            }
            catch
            { }
        }

        private void btnClearMasters_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.gblUserName.ToUpper() == "ADMIN" || Global.gblUserName.ToUpper() == "DIGIPOS")
                {
                    if (MessageBox.Show("Are you sure to clear all masters.", "Clear Masters", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string sQuery = "";
                        sQuery = "EXEC CLEARMASTERS";
                        Comm.fnExecuteNonQuery(sQuery);
                        Comm.MessageboxToasted("Clear Masters", "Masters Cleared Successfully");
                    }
                }
                else
                {
                    MessageBox.Show("You are not authorised to clear master details.", "Clear Master", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtBackUpPath1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBarcodePrefix_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sQuery = "";

            try
            {
                if (MessageBox.Show("Do you like to change default pricelist for cash customer", "DB Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sQuery = @"UPDATE TBLLEDGER SET PLID=1 WHERE LID=1000";

                    Comm.fnExecuteNonQuery(sQuery, false);
                }
            }
            catch
            { }

            try
            {
                if (MessageBox.Show("Do you like to change default pricelist for general customer", "DB Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sQuery = @"UPDATE TBLLEDGER SET PLID=1 WHERE LID=101";

                    Comm.fnExecuteNonQuery(sQuery, false);
                }
            }
            catch
            { }

            try
            {
                if (MessageBox.Show("Do you like to change default pricelist for general supplier", "DB Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sQuery = @"UPDATE TBLLEDGER SET PLID=1 WHERE LID=100";

                    Comm.fnExecuteNonQuery(sQuery, false);
                }
            }
            catch
            { }

            try
            {
                if (MessageBox.Show("Do you like to activate all tax parameter ledgers", "DB Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sQuery = @"update tblledger set ActiveStatus = 1 where TaxParameter <> 'DEFAULT' and TaxParameter <> ''";

                    Comm.fnExecuteNonQuery(sQuery, false);
                }
            }
            catch
            { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sQuery = "";

            try
            {
                if (MessageBox.Show("Do you like to change default pricelist for cash customer", "DB Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sQuery = @"UPDATE TBLLEDGER SET PLID=1 WHERE LID=1000";

                    Comm.fnExecuteNonQuery(sQuery, false);
                }
            }
            catch
            { }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string sQuery = "";

            try
            {
                if (MessageBox.Show("Do you like to remove credit from upi.", "DB Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sQuery = @"delete from tblCashDeskMaster where PaymentID = 3";

                    Comm.fnExecuteNonQuery(sQuery, false);
                }
            }
            catch
            { }
        }
    }
}
