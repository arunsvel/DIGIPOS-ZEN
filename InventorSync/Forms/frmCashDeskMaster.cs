using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using System.Runtime.InteropServices;
using InventorSync.JsonClass;
using InventorSync.InventorBL.Accounts;
using InventorSync.Forms;

namespace InventorSync
{
    // ======================================================== >>
    // Description:Color Creation
    // Developed By:Pramod Philip
    // Completed Date & Time: 09/09/2021 3.30 PM
    // Last Edited By:Anjitha k k
    // Last Edited Date & Time:01-March-2022 02:30 PM
    // ======================================================== >>

    public partial class frmCashDeskMaster : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmCashDeskMaster(int iColorID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

            iIDFromEditWindow = iColorID;
            bFromEditWindowColor = bFromEdit;
            CtrlPassed = Controlpassed;
            this.BackColor = Global.gblFormBorderColor;
            if (iColorID != 0)
            {
                LoadData(iColorID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtPaymentType.Focus();
            txtPaymentType.SelectAll();
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspInsertCashDeskMaster CashDeskinfo = new UspInsertCashDeskMaster();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetCashDeskIMasterInfo GetCashDesk = new UspGetCashDeskIMasterInfo();
        UspGetColorInfo GetColor = new UspGetColorInfo();
        clsColorMaster clsColor = new clsColorMaster();
        clsCashDeskMaster clsCashDeskMaster = new clsCashDeskMaster();
        UspGetMasterInfo GetMaster = new UspGetMasterInfo();
        clsMaster clsMaster = new clsMaster();
        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsLedger clsLedg = new clsLedger();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        string strCheck;
        Control ctrl;
        bool bFromEditWindowColor;
        Control CtrlPassed;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        //For Drag Form
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
        //For Help Using ToolTip
        private void txtColorName_Click(object sender, EventArgs e)
        {
            toolTipColor.SetToolTip(txtPaymentType, "Please specify the unique  Color");

        }
        private void txtColorHexCode_Click(object sender, EventArgs e)
        {
            toolTipColor.SetToolTip(txtLedger, "Please enter the  matched the Color HexCode");
            string strCondition = "";
                    strCondition = " and  accountgroupid in (16) ";

            string sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
                    " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 " + strCondition;
            frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromLedgerSearch, sQuery, "Anywhere|LedgerCode|LedgerName", this.Left + 155, this.Top + 20, 7, 0, "", 7, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
            frmN.MdiParent = this.MdiParent;
            frmN.Show();

        }
        private Boolean GetFromLedgerSearch(string sReturn)
        {
            try
            {
                DataTable dtSupp = new DataTable();

                string[] sCompSearchData = sReturn.Split('|');

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
         private void txtLedger_TextChanged(object sender, EventArgs e)
        {
            try
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
                        lblLID.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private int ConvertI32(decimal dVal)
        {
            try
            {
                return Convert.ToInt32(dVal);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
        }

        private void frmColorMaster_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();
                    this.Show();
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                }
                txtPaymentType.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Color......" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Cursor.Current = Cursors.Default;
        }
        private void frmColorMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtPaymentType.Text != "")
                    {
                        if (txtPaymentType.Text != strCheck)
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
                //    frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
                //    frmEdit.Show();
                //}
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                        Comm.ControlEnterLeave(txtPaymentType);
                        Application.DoEvents();

                        SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowColor == true)
                    {
                        try
                        {
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Color[" + txtPaymentType.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Color [" + txtPaymentType.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Shortcut keys not working properly  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    }
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
            Cursor.Current = Cursors.Default;
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
                    {
                        this.SelectNextControl(ctrl, false, false, false, false);
                    }
                    else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
                    }
                    else
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtColorHexCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtPaymentType.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtColorName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPaymentType, true);
        }
        private void txtColorName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPaymentType);
        }
        private void txtColorHexCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtLedger, true);
        }
        private void txtColorHexCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtLedger, false, false);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                SaveData();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Cursor.Current = Cursors.WaitCursor;
                if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Color[" + txtPaymentType.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Color [" + txtPaymentType.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed Delete...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Cursor.Current = Cursors.WaitCursor;
                frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
                //frmEdit.ShowDialog();
                frmEdit.Show();
                frmEdit.BringToFront();
                //this.Close();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to view edit window...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Cursor.Current = Cursors.Default;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPaymentType.Text != "")
                {
                    if (txtPaymentType.Text != strCheck)
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
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Close...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region "METHODS --------------------------------------------- >>"
        //Description : Validating the Mandatory Fields Before Save Functionality
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
        private bool IsValidate()
        {
            bool bValidate = true;
            if (txtPaymentType.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Color Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtPaymentType.Focus();
            }
            else
            {
                if (txtLedger.Text == "")
                    txtLedger.Text = "0";

                txtPaymentType.Text = txtPaymentType.Text.Replace("'", "\"");
            }
            return bValidate;
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetCashDesk.PaymentID = Convert.ToDecimal(iSelectedID);
            dtLoad = clsCashDeskMaster.GetCashDeskMaster(GetCashDesk);
            if (dtLoad.Rows.Count > 0)
            {
                txtPaymentType.Text = dtLoad.Rows[0]["PaymentType"].ToString();
                strCheck = dtLoad.Rows[0]["PaymentType"].ToString();
                txtLedger.Text = dtLoad.Rows[0]["LedgerID"].ToString();
                iAction = 1;
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                string[] strResult;
                string strRet = "";
                if (iAction == 0)
                {
                    CashDeskinfo.PaymentID = Comm.gfnGetNextSerialNo("tblCashDeskMaster", "PaymentID");
                    if (CashDeskinfo.PaymentID < 6)
                        CashDeskinfo.PaymentID = 6;
                }
                else
                    CashDeskinfo.PaymentID = Convert.ToDecimal(iIDFromEditWindow);
                CashDeskinfo.PaymentType = txtPaymentType.Text;
                DataTable dtUspColor = new DataTable();
                string d = lblLID.Text;
                decimal lid= decimal.Parse(lblLID.Text);
                CashDeskinfo.LedgerID = lid;
                strRet = clsCashDeskMaster.InsertUpdateDeleteCashDeskMaster(CashDeskinfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Color (" + txtPaymentType.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtPaymentType.Focus();
                            txtPaymentType.SelectAll();
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        CtrlPassed.Text = txtPaymentType.Text;
                        CtrlPassed.Tag = CashDeskinfo.PaymentID;

                        CtrlPassed.Focus();
                        this.Close();
                    }
                    else
                    {
                        ClearAll();
                        if (bFromEditWindowColor == true)
                        {
                            this.Close();
                        }

                    }
                    Comm.MessageboxToasted("", "Payment Type saved successfully");
                }
            }
        }
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            CashDeskinfo.PaymentID = Convert.ToDecimal(iIDFromEditWindow);
            CashDeskinfo.PaymentType = txtPaymentType.Text;
            CashDeskinfo.LedgerID = Decimal.Parse(lblLID.Text);
            GetMaster.TYPE = "COLOR";
            GetMaster.ID = Convert.ToInt32(CashDeskinfo.PaymentID);
            DataTable dtMaster = new DataTable();
            dtMaster = clsMaster.GetColumnIDsData(GetMaster);//Checking Color is Used in Item Master or not
            if (dtMaster.Rows.Count == 0)
            {
                strRet = clsCashDeskMaster.InsertUpdateDeleteCashDeskMaster(CashDeskinfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        ClearAll();
                }
                if (bFromEditWindowColor == true)
                {
                    this.Close();
                }
            }
            else
                MessageBox.Show("Hey! There are Items Associated with this Color [" + txtPaymentType.Text + "]. Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtPaymentType.Clear();
            txtLedger.Clear();
            btnDelete.Enabled = false;
            txtPaymentType.Focus();
        }
        #endregion
    }
}
