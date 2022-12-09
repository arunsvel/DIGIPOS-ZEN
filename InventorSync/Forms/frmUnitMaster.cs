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
using System.Runtime.InteropServices;

namespace DigiposZen
{
    public partial class frmUnitMaster : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description:Unit Creation
        // Developed By:Pramod Philip
        // Completed Date & Time:10/09/2021 6.00 PM 
        // Last Edited By:Anjitha k k
        // Last Edited Date & Time:01-March-2022 02:15 PM
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmUnitMaster(int iUnitID = 0, bool bFromEdit = false, Control Controlpassed = null)
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

                iIDFromEditWindow = iUnitID;
                bFromEditWindowUnit = bFromEdit;
                CtrlPassed = Controlpassed;
                this.BackColor = Global.gblFormBorderColor;
                if (iUnitID != 0)
                {
                    LoadData(iUnitID);
                }
                txtUnitName.Focus();
                txtUnitName.SelectAll();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Unit" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspInsertUnitMasterInfo UnitInfo = new UspInsertUnitMasterInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetUnitInfo GetUnit = new UspGetUnitInfo();
        clsUnitMaster  clsUnit = new clsUnitMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        string strCheck;
        Control ctrl;
        bool bFromEditWindowUnit;
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
        private void txtUnitName_Click(object sender, EventArgs e)
        {
            toolTipUnit.SetToolTip(txtUnitName, "Please specify unique Unit Name");
        }
        private void txtUnitShortName_Click(object sender, EventArgs e)
        {
            toolTipUnit.SetToolTip(txtUnitShortName, "Specify unit Shortname to show in print and specified area.");
        }

        private void frmUnitMaster_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();
                    this.Show();
                    Application.DoEvents();
                }
                txtUnitName.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load ..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void frmUnitMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtUnitName.Text != "")
                    {
                        if (txtUnitName.Text != strCheck)
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
                        Comm.ControlEnterLeave(txtUnitName);
                        Comm.ControlEnterLeave(txtUnitShortName);
                        Application.DoEvents();

                        SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowUnit == true)
                    {
                        try
                        {
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Unit[" + txtUnitName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Unit [" + txtUnitName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Shortcut Keys not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
                    {
                        this.SelectNextControl(ctrl, false, false, false, false);
                    }
                    else if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
                        txtUnitShortName.SelectAll();
                    }
                    else
                        return;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press not working properly...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtUnitShortName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtUnitName.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    Comm.ControlEnterLeave(txtUnitShortName, false, false);
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save ? " + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtUnitName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUnitName.Text))
            {
                txtUnitShortName.Clear();
            }
        }
        private void txtUnitName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUnitName, true);
        }
        private void txtUnitName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtUnitName.Text.Length > 4)
                {
                    if (txtUnitShortName.Text.Trim() == "")
                        txtUnitShortName.AppendText(txtUnitName.Text.Substring(0, 4));
                }
                else
                {
                    if (txtUnitShortName.Text.Trim() == "")
                        txtUnitShortName.AppendText(txtUnitName.Text);
                }
                Comm.ControlEnterLeave(txtUnitName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  append Unit Name to Unit Shortname...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtUnitShortName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUnitShortName, true);
        }
        private void txtUnitShortName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUnitShortName, false, false);
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
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Unit[" + txtUnitName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Unit [" + txtUnitName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
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
            if (txtUnitName.Text != "")
            {
                if (txtUnitName.Text != strCheck)
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
            if (txtUnitName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Unit Name ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUnitName.Focus();
            }
            //else if (txtUnitShortName.Text.Trim() == "")
            //{
            //    bValidate = false;
            //    MessageBox.Show("Please enter Unit Short Name ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    txtUnitShortName.Focus();
            //}
            else
            {
                txtUnitName.Text = txtUnitName.Text.Replace("'", "\"");
            }
            return bValidate;
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetUnit.UnitID= Convert.ToDecimal(iSelectedID);
            GetUnit.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsUnit.GetUnitMaster(GetUnit);
            if (dtLoad.Rows.Count > 0)
            {
                txtUnitName.Text = dtLoad.Rows[0]["UnitName"].ToString();
                strCheck = dtLoad.Rows[0]["UnitName"].ToString();
                txtUnitShortName.Text = dtLoad.Rows[0]["UnitShortName"].ToString();
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

                DataTable dtUspUnit = new DataTable();
                if (iAction == 0)
                {
                    UnitInfo.UnitID = Comm.gfnGetNextSerialNo("tblUnit", "UnitID");
                    if (UnitInfo.UnitID < 6)
                        UnitInfo.UnitID = 6;
                }
                else
                    UnitInfo.UnitID = Convert.ToDecimal(iIDFromEditWindow);
                UnitInfo.UnitName = txtUnitName.Text;
                if (txtUnitShortName.Text.Trim() == "")
                {
                    if (txtUnitName.Text.Length > 4)
                        txtUnitShortName.Text = txtUnitName.Text.Substring(0, 4);
                    else
                        txtUnitShortName.Text = txtUnitName.Text;
                }
                UnitInfo.UnitShortName = txtUnitShortName.Text;
                UnitInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                    strRet = clsUnit.InsertUpdateDeleteUnitMaster(UnitInfo, iAction);
                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                if (strResult[1].ToString().Contains("UK_tblUnitShortName"))
                                {
                                    MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Unit short name (" + txtUnitShortName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    txtUnitShortName.Focus();
                                    txtUnitShortName.SelectAll();
                                }
                                else
                                {
                                    MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Unit name (" + txtUnitName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    txtUnitName.Focus();
                                    txtUnitName.SelectAll();
                                }
                            }
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Save...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                        {
                         if (CtrlPassed is TextBox)
                         {
                            CtrlPassed.Text = txtUnitName.Text;
                            CtrlPassed.Tag = UnitInfo.UnitID;
                            CtrlPassed.Focus();
                            this.Close();
                         }
                         else if (CtrlPassed is ComboBox)
                         {
                            DataTable dtBrand = Comm.fnGetData("SELECT UnitID,UnitShortName FROM tblunit  WHERE TenantID = " + Global.gblTenantID + " ORDER BY UnitShortName Asc").Tables[0];
                            ((ComboBox)CtrlPassed).DataSource = dtBrand;
                            ((ComboBox)CtrlPassed).DisplayMember = "UnitShortName";
                            ((ComboBox)CtrlPassed).ValueMember = "UnitID";
                            ((ComboBox)CtrlPassed).SelectedValue = UnitInfo.UnitID;
                            ((ComboBox)CtrlPassed).Tag = UnitInfo.UnitID;

                            CtrlPassed.Focus();
                            this.Close();
                         }
                        }
                        else
                        {
                            ClearAll();
                            if (bFromEditWindowUnit == true)
                            {
                                this.Close();
                            }
                        }
                         Comm.MessageboxToasted("Unit", "Unit saved successfully");
                    }
            }
        }
        //Description :  Delete Data from Unit table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtUspUnit = new DataTable();
            UnitInfo.UnitID = Convert.ToDecimal(iIDFromEditWindow);
            UnitInfo.UnitName = txtUnitName.Text;
            UnitInfo.UnitShortName = txtUnitShortName.Text;
            strRet = clsUnit.InsertUpdateDeleteUnitMaster(UnitInfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                   if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                      MessageBox.Show("Hey! There are Items Associated with this Unit [" + txtUnitName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (bFromEditWindowUnit == true)
            {
                this.Close();
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtUnitName.Clear();
            txtUnitShortName.Clear();
            btnDelete.Enabled = false;
            txtUnitName.Focus();
        }
        #endregion
    }
}
