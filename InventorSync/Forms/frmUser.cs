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
using DigiposZen.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.Runtime.InteropServices;

namespace DigiposZen
{
    public partial class frmUser : Form, IMessageFilter
    {

        // ======================================================== >>
        // Description:  User Creation          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 17/03/2022   & 10:00 AM
        // Last Edited By:  Anjitha        
        // Last Edited Date & Time: 18/02/2022
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmUser(int iUser = 0, bool bFromEdit = false,Control Controlpassed = null)
        {
            try
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

                iIDFromEditWindow = iUser;
                bFromEditWindowUser = bFromEdit;
                this.BackColor = Global.gblFormBorderColor;
                if (iIDFromEditWindow != 0)
                {
                    LoadUserGroup();
                    LoadStaffLedger();
                    toggleViewCounterMgmt();
                    LoadCompanies();
                    LoadData(iIDFromEditWindow);
                    txtUserName.Focus();
                    txtUserName.SelectAll();
                }
                else
                {
                    btnDelete.Enabled = false;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
              MessageBox.Show("Failed to load Ledger" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES  -------------------------------------------- >>"
        //Info
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetCostCentreInfo GetCostCentreinfo = new UspGetCostCentreInfo();
        UspUserMasterInsertInfo UserInfo = new UspUserMasterInsertInfo();
        UspGetUserMasterInfo GetUsrInfo = new UspGetUserMasterInfo();

        //Class
        clsCostCentre clsCostCntr = new clsCostCentre();
        clsUser clsUsr = new clsUser();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        int intCountermgmt;
        Control ctrl;
        string strCheck;
        string strCostcntr;
        bool bFromEditWindowUser;
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
        private void txtUserName_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtUserName, "Please enter User Name");
        }
        private void txtpwd_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtUserName, "Please enter Password");
        }
        private void txtcpwd_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtUserName, "Please enter Confirm Password");
        }
        private void txtPin_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtUserName, "Please enter Pin");
        }
        private void txtHintQuest_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtUserName, "Please enter Hint question");
        }
        private void txtHintAnswer_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtHintAnswer, "Please enter Hint answer");
        }
        private void txtCCntr_Click(object sender, EventArgs e)
        {
            toolUser.SetToolTip(txtCCntr, "Please Select Cost Centre");
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
                        SendKeys.Send("{F4}");
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
        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtUserName.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    cboGroup.Focus();
                    SendKeys.Send("{F4}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cboLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    toggleCountermgmt.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                   // SaveData();
                }
                else if (e.KeyCode == Keys.F3)
                {
                    btnAddLedg.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void toggleNextLogin_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtCCntr.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    toggleCountermgmt.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void toggleCountermgmt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    toggleNextLogin.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (toggleCountermgmt.ToggleState == ToggleButtonState.Active)
                    {
                        cboLedger.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else if (toggleCountermgmt.ToggleState == ToggleButtonState.Inactive)
                    {
                        SaveData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtPin_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtHintAnswer.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    txtCCntr.Focus();
                    SendKeys.Send("+{DOWN}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtPin_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtCCntr_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtPin.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    toggleNextLogin.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    string sQuery = "SELECT CCID,CCName FROM tblCostCentre WHERE TenantID = " + Global.gblTenantID + "";
                    new frmCompactCheckedListSearch(GetFromCheckedList, sQuery, "CCName", txtCCntr.Location.X + 475, txtCCntr.Location.Y + 300, 0, 2, txtCCntr.Text, 0, 0, "", txtCCntr.Text,null,"Cost Centre", "frmCostCentre").ShowDialog();
                    toggleNextLogin.Focus();
                }
                else if (e.KeyCode == Keys.F3)
                {
                    btnAddCcostcntr.PerformClick();
                }
                else if (e.KeyCode == Keys.F4)
                {
                    btnEditCcostcntr.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtUserName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUserName, true);
        }
        private void txtUserName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUserName, false, false);
        }
        private void cboGroup_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboGroup, true);
        }
        private void cboGroup_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboGroup, false, false);
        }
        private void txtpwd_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtpwd, true);
        }
        private void txtpwd_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtpwd, false, false);
        }
        private void txtcpwd_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtcpwd, true);
        }
        private void txtcpwd_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtcpwd, false, false);
            if (txtpwd.Text != txtcpwd.Text)
            {
                MessageBox.Show("Password Don't Match.Try Again", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtcpwd.Focus();
            }
        }
        private void txtHintQuest_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHintQuest, true);
        }
        private void txtHintQuest_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHintQuest, false, false);
        }
        private void txtHintAnswer_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHintAnswer, true);
        }
        private void txtHintAnswer_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHintAnswer, false, false);
        }
        private void txtPin_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPin, true);
        }
        private void txtPin_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPin, false, false);
        }
        private void txtCCntr_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCCntr, true);
        }
        private void txtCCntr_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCCntr, false, false);
        }
        private void toggleNextLogin_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(toggleNextLogin, true);
        }
        private void toggleNextLogin_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(toggleNextLogin, false, false);
        }
        private void toggleCountermgmt_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(toggleCountermgmt, true);
        }
        private void toggleCountermgmt_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(toggleCountermgmt, false, false);
        }
        private void cboLedger_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboLedger, true);
        }
        private void cboLedger_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboLedger, false, false);
        }

        private void toggleCountermgmt_ToggleStateChanged(object sender, ToggleStateChangedEventArgs e)
        {
            toggleViewCounterMgmt();
        }
        private void cboLedger_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboLedger.Tag = cboLedger.SelectedValue;
        }

        private void frmUser_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    this.Show();
                    Application.DoEvents();
                    LoadUserGroup();
                    LoadStaffLedger();
                    toggleViewCounterMgmt();
                    SetDefaultValue();
                    cboGroup.SelectedIndex = 0;
                    cboLedger.SelectedIndex = 0;
                    txtUserName.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load User  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void frmUser_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtUserName.Text != "")
                    {
                        if (txtUserName.Text != strCheck)
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
                else if (e.KeyCode == Keys.F3)//Find
                {
                    //frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper());
                    //frmEdit.Show();
                }
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                            SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowUser == true)
                    {
                        try
                        {
                            if (iIDFromEditWindow > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete User[" + txtUserName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                             else
                              MessageBox.Show("Default User [" + txtUserName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to Delete " + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
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
        }

        private void btnAddCcostcntr_Click(object sender, EventArgs e)
        {
            try
            {
                string CC = txtCCntr.Text;
                string CCIDs = Convert.ToString(txtCCntr.Tag);
                frmCostCentre frmCcntr = new frmCostCentre(0, false, txtCCntr);
                frmCcntr.ShowDialog();

                if (CC.Trim().Length > 0)
                {
                    txtCCntr.Text = CC + "," + txtCCntr.Text;
                    txtCCntr.Tag = CCIDs + "," + txtCCntr.Tag;
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEditCcostcntr_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Cannot Handle the Process, because Multi selected Cost Centre Can't Edit !!s");
                txtCCntr.Focus();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnAddLedg_Click(object sender, EventArgs e)
        {
            try
            {
                frmLedger frmLed = new frmLedger(0, false, 20, "", cboLedger);
                frmLed.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEditLedg_Click(object sender, EventArgs e)
        {
            try
            {
                frmLedger frmLed = new frmLedger(Convert.ToInt32(cboLedger.Tag), true, 20, "", cboLedger);
                frmLed.Show();
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
                SaveData();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            if (iIDFromEditWindow > 5)
            {
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete User[" + txtUserName.Text + "] Permanently ?", Global.gblMessageCaption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    try
                    {
                        DeleteData();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            else
                MessageBox.Show("Default User [" + txtUserName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor.Current = Cursors.Default;
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
                frmEdit.ShowDialog();
                this.Visible = false;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Find..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtUserName.Text != "")
            {
                if (txtUserName.Text != strCheck)
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
        #endregion

        #region "METHODS ----------------------------------------------- >>"
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
            if (txtUserName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter User Name ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUserName.Focus();
            }
            else if (txtpwd.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Password", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtpwd.Focus();
            }
            else if (txtcpwd.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Confirm Password", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtcpwd.Focus();
            }
            else if(txtpwd.Text != txtcpwd.Text)
            {
                bValidate = false;
                MessageBox.Show("Password Don't Match.Try Again", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtcpwd.Focus();
            }
            return bValidate;
        }
        //Description : Toggle State Change
        private void toggleViewCounterMgmt()
        {
            if (toggleCountermgmt.ToggleState == ToggleButtonState.Active)
            {
                intCountermgmt = 1;
                gpbStaffledger.Visible = true;
                cboLedger.Focus();
                SendKeys.Send("{F4}");
            }
            if (toggleCountermgmt.ToggleState == ToggleButtonState.Inactive)
            {
                intCountermgmt = 0;
                gpbStaffledger.Visible = false;
            }
        }
        //Description : Set Default Value when Control is Empty
        private void SetDefaultValue()
        {
            if (string.IsNullOrEmpty(txtCCntr.Text))
            {
                txtCCntr.Tag = 1;
                txtCCntr.Text = Comm.fnGetData("Select CCName From tblCostCentre Where CCID = '" + txtCCntr.Tag + "'").Tables[0].Rows[0][0].ToString();
                strCostcntr = txtCCntr.Text;
            }
        }
        //Description : Get all  and  Selected Category to show Checked Compact List
        private string GetCostCenterAsperIDs(string sIDs = "")
        {
            string sRetResult = "";
            DataTable dtData = new DataTable();
            GetCostCentreinfo.CCID = 0;
            GetCostCentreinfo.CCIDs = sIDs;
            GetCostCentreinfo.TenantID = Global.gblTenantID;
            dtData = clsCostCntr.GetCostCentre(GetCostCentreinfo);
            if (dtData.Rows.Count > 0)
            {
                sRetResult = dtData.Rows[0][0].ToString();
            }
            return sRetResult;
        }
        //Description : Set  Checked Cost Center to Cost Center TextBox 
        private Boolean GetFromCheckedList(string sSelIDs)
        {
            txtCCntr.Tag = sSelIDs;
            txtCCntr.Text = GetCostCenterAsperIDs(sSelIDs);
            return true;
        }
        //Description : Fill Companies to Checked list box
        private void LoadCompanies()
        {
            DataTable dtGroup = new DataTable();
            dtGroup = Comm.fnGetData("Select CompanyID,CompanyCode + ' - ' + companyname as company from startup.dbo.tblCompany WHERE Active = 1 ORDER BY CompanyCode Asc").Tables[0];
            chkCompanies.DataSource = dtGroup;
            chkCompanies.DisplayMember = "company";
            chkCompanies.ValueMember = "CompanyID";
        }
        //Description : Fill User Group to Combobox
        private void LoadUserGroup()
        {
            DataTable dtGroup = new DataTable();
            dtGroup = Comm.fnGetData("Select GroupName,ID from tblUserGroupMaster WHERE TenantID = " + Global.gblTenantID + " ORDER BY ID Asc").Tables[0];
            cboGroup.DataSource = dtGroup;
            cboGroup.DisplayMember = "GroupName";
            cboGroup.ValueMember = "ID";
        }
        //Description : Fill Staff Ledger to Combobox
        private void LoadStaffLedger()
        {
            DataTable dtGroup = new DataTable();
            dtGroup = Comm.fnGetData("Select 0 as LID,'<None>' as LName FROM tblLedger UNION SELECT LID,LName from tblledger where AccountGroupID = 20 AND TenantID = " + Global.gblTenantID + " ORDER BY LID Asc").Tables[0];
            cboLedger.DataSource = dtGroup;
            cboLedger.DisplayMember = "LName";
            cboLedger.ValueMember = "LID";
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataTable dtLoad = new DataTable();
                GetUsrInfo.UserID = Convert.ToInt32(iSelectedID);
                dtLoad = clsUsr.GetUserMaster(GetUsrInfo);
                if (dtLoad.Rows.Count > 0)
                {
                    txtUserName.Text = dtLoad.Rows[0]["UserName"].ToString();
                    strCheck = dtLoad.Rows[0]["UserName"].ToString();
                    cboGroup.SelectedValue= dtLoad.Rows[0]["GroupID"].ToString();
                    txtpwd.Text = dtLoad.Rows[0]["Pwd"].ToString();
                    txtpwd.Tag = dtLoad.Rows[0]["StartupUserID"].ToString();
                    txtcpwd.Text = dtLoad.Rows[0]["Pwd"].ToString();
                    txtHintQuest.Text = dtLoad.Rows[0]["HintQuestion"].ToString();
                    txtHintAnswer.Text = dtLoad.Rows[0]["HintAnswer"].ToString();
                    txtPin.Text= dtLoad.Rows[0]["PIN"].ToString();
                    cboLedger.Tag= dtLoad.Rows[0]["UserLedgerID"].ToString();
                    if (dtLoad.Rows[0]["UserLedgerID"].ToString() != "")
                        cboLedger.SelectedValue = Convert.ToDecimal(dtLoad.Rows[0]["UserLedgerID"].ToString());

                    string strCostcntrIds = "1";
                    if (dtLoad.Rows[0]["UserLedgerID"].ToString() != "")
                        strCostcntrIds = dtLoad.Rows[0]["CCIDs"].ToString().Substring(1, dtLoad.Rows[0]["CCIDs"].ToString().Length - 2);
                    txtCCntr.Tag = strCostcntrIds;
                    
                    txtCCntr.Text = Comm.fnGetData("EXEC UspGetCheckedList '" + txtCCntr.Tag + "'," + Global.gblTenantID + ",'COSTCENTRE'").Tables[0].Rows[0][0].ToString();



                    if (Convert.ToInt32(dtLoad.Rows[0]["Status"].ToString()) == 1)
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    if (Convert.ToInt32(dtLoad.Rows[0]["changepwdonlogon"].ToString()) == 1)
                        toggleNextLogin.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        toggleNextLogin.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    
                          if (Convert.ToInt32(dtLoad.Rows[0]["ActiveCounterID"].ToString()) == 1)
                        toggleCountermgmt.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        toggleCountermgmt.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    
                    iAction = 1;
                }
                Cursor.Current = Cursors.Default; ;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load Data from Edit" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                 string[] strResult;
                 string strRet = "";
                 int iActive = 1;
                 int iActiveLogon = 0;
                 int iActiveCountermgmt = 0;

                if (iAction == 0)
                {
                    UserInfo.UserID = Comm.gfnGetNextSerialNo("tblUserMaster", "UserID");
                    if (UserInfo.UserID < 6)
                        UserInfo.UserID = 6;
                }
                else
                    UserInfo.UserID = iIDFromEditWindow;

                if (txtpwd.Tag == null) txtpwd.Tag = "0";
                if (txtpwd.Tag.ToString() == "") txtpwd.Tag = "0";

                UserInfo.StartupUserID = Convert.ToInt32(txtpwd.Tag.ToString().TrimStart().TrimEnd());

                UserInfo.UserName = txtUserName.Text.TrimStart().TrimEnd();
                UserInfo.Pwd= txtpwd.Text.TrimStart().TrimEnd();
                UserInfo.GroupID = Convert.ToInt32(cboGroup.SelectedValue);
                if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActive = 1;
                else
                    iActive = 0;
                UserInfo.Status = iActive;
                if (toggleNextLogin.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActiveLogon = 1;
                else
                    iActiveLogon = 0;
                UserInfo.changepwdonlogon = iActiveLogon;
                UserInfo.CostCentre = txtCCntr.Text;
                UserInfo.HintAnswer = txtHintAnswer.Text;
                UserInfo.HintQuestion = txtHintQuest.Text;
                UserInfo.WorkingDays = "0";
                UserInfo.WorkFrom = Convert.ToDateTime("01-01-1900");
                UserInfo.WorkTo = Convert.ToDateTime("01-01-1900");
                UserInfo.godown = "";

                string strFirstCCID = "";
                string strCCID = Convert.ToString(txtCCntr.Tag);
                if (txtCCntr.Tag != "0")
                {
                    int index = strCCID.IndexOf(',');
                    if (index == -1)
                        strFirstCCID = strCCID.Trim();
                    else
                        strFirstCCID = strCCID.Substring(0, index);
                }
                if (strFirstCCID == "")
                    strFirstCCID = "0";
                UserInfo.SelectedCCID = Convert.ToDecimal(strFirstCCID);
                UserInfo.SystemName= Environment.MachineName;
                UserInfo.LastUpdateDate=DateTime.Today;
                UserInfo.LastUpdateTime= DateTime.Now;
                UserInfo.OrderVchtypeIDs = "";
                UserInfo.SalesVchtypeIDs = "";
                UserInfo.SalesReturnVchtypeIDs = "";
                UserInfo.AccountsVchtypeIDs = "";
                if (cboLedger.Tag == "")
                    cboLedger.Tag = 0;
                UserInfo.UserLedgerID = Convert.ToDecimal(cboLedger.Tag);
                if (toggleCountermgmt.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActiveCountermgmt = 1;
                else
                    iActiveCountermgmt = 0;
                UserInfo.ActiveCounterID = iActiveCountermgmt;
                if (txtPin.Text == "")
                    txtPin.Text = "0";
                UserInfo.PIN = Convert.ToDecimal(txtPin.Text);
                if (txtCCntr.Text == "")
                    txtCCntr.Tag = "";
                UserInfo.CCIDs = "," + txtCCntr.Tag + ",";
                strRet = clsUsr.InsertUpdateDeleteUserMaster(UserInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                              MessageBox.Show("Duplicate Entry, User has restricted to enter duplicate values in the User Name(" + txtUserName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                              txtUserName.Focus();
                               txtUserName.SelectAll();
                        }
                        else
                             MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    else
                          ClearAll();
                          if (bFromEditWindowUser == true)
                          {
                              this.Close();
                          }
                    Comm.MessageboxToasted("User", "User saved successfully");
                }
            }
        }
        //Description :  Delete Data from Ledger table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            int iActive = 1;
            int iActiveLogon = 0;
            int iActiveCountermgmt = 0;
            iAction = 2;

            UserInfo.UserID = iIDFromEditWindow;
            UserInfo.UserName = txtUserName.Text.TrimStart().TrimEnd();
            UserInfo.Pwd = txtpwd.Text.TrimStart().TrimEnd();
            UserInfo.GroupID = Convert.ToInt32(cboGroup.SelectedValue);
            if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActive = 1;
            else
                iActive = 0;
            UserInfo.Status = iActive;
            if (toggleNextLogin.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActiveLogon = 1;
            else
                iActiveLogon = 0;
            UserInfo.changepwdonlogon = iActiveLogon;
            UserInfo.CostCentre = txtCCntr.Text;
            UserInfo.HintAnswer = txtHintAnswer.Text;
            UserInfo.HintQuestion = txtHintQuest.Text;
            UserInfo.WorkingDays = "0";
            UserInfo.WorkFrom = Convert.ToDateTime("01-01-1900");
            UserInfo.WorkTo = Convert.ToDateTime("01-01-1900");
            UserInfo.godown = "";
            UserInfo.SelectedCCID = 0;
            UserInfo.SystemName = Environment.MachineName;
            UserInfo.LastUpdateDate = DateTime.Today;
            UserInfo.LastUpdateTime = DateTime.Now;
            UserInfo.OrderVchtypeIDs = "";
            UserInfo.SalesVchtypeIDs = "";
            UserInfo.SalesReturnVchtypeIDs = "";
            UserInfo.AccountsVchtypeIDs = "";
            UserInfo.UserLedgerID = Convert.ToDecimal(cboLedger.SelectedValue);
            if (toggleCountermgmt.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActiveCountermgmt = 1;
            else
                iActiveCountermgmt = 0;
            UserInfo.ActiveCounterID = iActiveCountermgmt;
            UserInfo.PIN = Convert.ToDecimal(txtPin.Text);
            if (txtCCntr.Text == "")
                txtCCntr.Tag ="";
            UserInfo.CCIDs = "," + txtCCntr.Tag + ",";
            strRet = clsUsr.InsertUpdateDeleteUserMaster(UserInfo, iAction);

            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are entries Associated with this User [" + txtUserName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (Convert.ToInt32(strRet) == -1)
                    MessageBox.Show("Failed to Delete...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    ClearAll();
            }
            if (bFromEditWindowUser == true)
            {
                this.Close();
            }

        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtUserName.Text = "";
            cboGroup.SelectedIndex = 0;
            cboLedger.SelectedIndex = 0;
            txtpwd.Text = "";
            txtpwd.Tag = "";

            txtcpwd.Text = "";
            txtHintQuest.Text = "";
            txtHintAnswer.Text = "";
            txtPin.Text = "0";
            txtCCntr.Tag = 1;
            cboLedger.Tag = 0;
            txtCCntr.Text = strCostcntr;
            togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            toggleCountermgmt.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            toggleNextLogin.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            txtUserName.Focus();
        }
        #endregion

        private void gboxMain_Enter(object sender, EventArgs e)
        {

        }
    }
}

