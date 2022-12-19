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
using System.Runtime.InteropServices;

namespace DigiposZen
{
    public partial class frmState : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description:             State Creation Creation
        // Developed By:            Anjitha K K
        // Completed Date & Time:   25/02/2022 02.30 PM
        // Last Edited By:          
        // Last Edited Date & Time: 
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        string olddata = "";
        string newdata = "";
        string oldvalue = "";

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmState(int iStateID = 0,bool bFromEdit = false)
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
                lblSave.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);
                lblDelete.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);
                lblFind.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);

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

            iIDFromEditWindow = iStateID;
            bFromEditWindowState = bFromEdit;
            LoadCountry();
            this.BackColor = Global.gblFormBorderColor;
            if (iStateID != 0)
            {
                LoadData(iStateID);
            }
            else
            {
                cboStateType.SelectedIndex = 0;
                cboCountry.SelectedIndex = 55;
                btnDelete.Enabled = false;
            }
            txtStateCode.Focus();
            txtStateCode.SelectAll();
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES  -------------------------------------------- >>"
        //info
        UspGetStateInfo GetStateinfo = new UspGetStateInfo();
        UspInsertStateInfo Stateinfo = new UspInsertStateInfo();

        //Class
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsState clsStat = new clsState();

        bool dragging = false, bFromEditWindowState;
        int xOffset = 0, yOffset = 0, iAction=0, iIDFromEditWindow;
        string strCheck;
        Control ctrl;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        //drag Form 
        private void tlpHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpHeading_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpHeading_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }
        private void lblHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void lblHeading_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;

            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void lblHeading_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }
        //For Help
        private void txtStateCode_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(txtStateCode, "Please specify the unique  State Code");
        }
        private void txtStateName_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(txtStateName, "Please specify the unique  State Name");
        }
        private void cboStateType_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(cboStateType, "Please select a particular State Type");
        }
        private void cboCountry_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(cboCountry, "Please select a Particular Country");
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
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
                        SendKeys.Send("{F4}");
                    }
                    else if (e.KeyCode == Keys.Up && e.Control)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);

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
        private void cboCountry_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    cboStateType.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    try
                    {
                        SaveData();
                        cboCountry.SelectedIndex = 55;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to Save ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cboCountry_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                SaveData();
            }
        }
        private void frmState_Load(object sender, EventArgs e)
        {
        }
        private void frmState_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtStateCode.Text != "")
                    {
                        if (txtStateCode.Text != strCheck)
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
                        Comm.ControlEnterLeave(txtStateCode);
                        Comm.ControlEnterLeave(txtStateName);
                        Application.DoEvents();

                        SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowState == true)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete State Code[" + txtStateCode.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default State [" + txtStateName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            Cursor.Current = Cursors.Default;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Shortcut keys not working properly  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                        }
                    }
                }
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
        // For Casing
        private void txtStateCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtStateCode, true);
        }
        private void txtStateName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtStateName, true);
        }
        private void cboStateType_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboStateType, true);
        }
        private void cboCountry_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboCountry, true);
        }
        private void txtStateCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtStateCode);
        }
        private void txtStateName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtStateName);
        }
        private void cboStateType_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboStateType);
        }
        private void cboCountry_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboCountry);
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
                MessageBox.Show("Failed to Sve" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (iIDFromEditWindow > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete State Code[" + txtStateCode.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                       DeleteData();
                       Comm.writeuserlog(Common.UserActivity.Delete_Entry, newdata, olddata, "Deleted " + Stateinfo.State, 518, 518, Stateinfo.StateCode, Comm.ToInt32(Stateinfo.StateId), "State");

                    }
                }
                else
                    MessageBox.Show("Default State [" + txtStateCode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed Delete" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
            frmEdit.Show();
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtStateCode.Text != "")
            {
                if (txtStateCode.Text != strCheck)
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
            if (txtStateCode.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter State Code", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtStateCode.Focus();
            }
            else if (txtStateName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter State Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtStateName.Focus();
            }
            return bValidate;
        }
        //Description : Load Country to Combobox
        private void LoadCountry()
        {
            DataTable dtCountry = new DataTable();
            dtCountry = Comm.fnGetData("SELECT CountryID,Country From tblCountry ORDER BY Country Asc").Tables[0];
            cboCountry.DataSource = dtCountry;
            cboCountry.DisplayMember = "Country";
            cboCountry.ValueMember = "CountryID";
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetStateinfo.StateId = Convert.ToDecimal(iSelectedID);
            GetStateinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsStat.GetStates(GetStateinfo);
            if (dtLoad.Rows.Count > 0)
            {
                txtStateCode.Text = dtLoad.Rows[0]["StateCode"].ToString();
                strCheck = dtLoad.Rows[0]["StateCode"].ToString();
                txtStateName.Text = dtLoad.Rows[0]["State"].ToString();
                cboStateType.Text = dtLoad.Rows[0]["StateType"].ToString();
                cboCountry.SelectedValue = dtLoad.Rows[0]["CountryID"].ToString();
                iAction = 1;
            }
            oldvalue = txtStateCode.Text;
            olddata = "StateCode:" + txtStateCode.Text + ",State:" + txtStateName.Text + ",StateType:" + cboStateType.Text + "Country:" + cboCountry.Text;

        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                newdata = "StateCode:" + txtStateCode.Text + ",State:" + txtStateName.Text + ",StateType:" + cboStateType.Text + "Country:" + cboCountry.Text;

                string[] strResult;
                string strRet = "";

                if (iAction == 0)
                {
                    Stateinfo.StateId = Comm.gfnGetNextSerialNo("tblStates", "StateID");
                    if (Stateinfo.StateId < 6)
                        Stateinfo.StateId = 6;
                }
                else
                Stateinfo.StateId = Convert.ToDecimal(iIDFromEditWindow);
                Stateinfo.StateCode = txtStateCode.Text.TrimStart().TrimEnd();
                Stateinfo.State = txtStateName.Text.TrimStart().TrimEnd();
                Stateinfo.StateType =Convert.ToString(cboStateType.SelectedItem);
                Stateinfo.Country = cboCountry.Text;
                Stateinfo.CountryID =Convert.ToDecimal(cboCountry.SelectedValue);
                Stateinfo.SystemName = Environment.MachineName;
                Stateinfo.UserID = Convert.ToDecimal(Global.gblUserID);
                Stateinfo.LastUpdateDate = DateTime.Today;
                Stateinfo.LastUpdateTime = DateTime.Now;
                Stateinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                strRet = clsStat.InsertUpdateDeleteStates(Stateinfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            if (strResult[1].ToString().Contains("IX_tblStates"))//StateCode
                            {
                                MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the State Code(" + txtStateCode.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtStateCode.Focus();
                                txtStateCode.SelectAll();
                            }
                            else
                            {
                                MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the State Name(" + txtStateName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtStateName.Focus();
                                txtStateName.SelectAll();
                            }
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ClearAll();
                        if (bFromEditWindowState == true)
                        {
                            this.Close();
                        }
                        Comm.MessageboxToasted("State", "State Saved Successfully");
                        if (iIDFromEditWindow > 0)
                        {

                            Comm.writeuserlog(Common.UserActivity.UpdateEntry, newdata, olddata, "Update " + oldvalue + " State to " + Stateinfo.State, 518, 518, Stateinfo.StateCode, Comm.ToInt32(Stateinfo.StateId), "State");

                        }
                        else
                        {

                            Comm.writeuserlog(Common.UserActivity.new_Entry, newdata, olddata, "Created " + Stateinfo.State, 518, 518, Stateinfo.StateCode, Comm.ToInt32(Stateinfo.StateId), "State");

                        }
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save! ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        ClearAll();
                        if (bFromEditWindowState == true)
                        {
                            this.Close();
                        }
                        Comm.MessageboxToasted("State", "State Saved Successfully");

                        if (iIDFromEditWindow > 0)
                        {

                            Comm.writeuserlog(Common.UserActivity.UpdateEntry, newdata, olddata, "Update " + oldvalue + " State to " + Stateinfo.State, 518, 518, Stateinfo.StateCode, Comm.ToInt32(Stateinfo.StateId), "State");

                        }
                        else
                        {

                            Comm.writeuserlog(Common.UserActivity.new_Entry, newdata, olddata, "Created " + Stateinfo.State, 518, 518, Stateinfo.StateCode, Comm.ToInt32(Stateinfo.StateId), "State");

                        }
                    }
                }
            }
        }
        //Description :  Delete Data from State table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            Stateinfo.StateId = iIDFromEditWindow;
            Stateinfo.StateCode = txtStateCode.Text.TrimStart().TrimEnd();
            Stateinfo.State = txtStateName.Text.TrimStart().TrimEnd();
            Stateinfo.StateType = cboStateType.Text.TrimStart().TrimEnd();
            Stateinfo.Country = cboCountry.SelectedText;
            Stateinfo.CountryID = Convert.ToDecimal(cboCountry.SelectedValue);
            Stateinfo.SystemName = Environment.MachineName;
            Stateinfo.UserID = Convert.ToDecimal(Global.gblUserID);
            Stateinfo.LastUpdateDate = DateTime.Today;
            Stateinfo.LastUpdateTime = DateTime.Now;
            Stateinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            strRet = clsStat.InsertUpdateDeleteStates(Stateinfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are entries Associated with this State [" + txtStateName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (Convert.ToInt32(strRet) == -1)
                    MessageBox.Show("Failed to Delete ?", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    ClearAll();
            }
            if (bFromEditWindowState == true)
            {
                this.Close();
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtStateCode.Text = "";
            txtStateName.Text = "";
            cboStateType.Text = "";
            cboStateType.SelectedIndex = 0;
            cboCountry.SelectedIndex = 55;
            txtStateCode.Focus();
        }
        #endregion
    }
}
