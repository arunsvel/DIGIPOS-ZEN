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

namespace InventorSync
{
    // ======================================================== >>
    // Description:Color Creation
    // Developed By:Pramod Philip
    // Completed Date & Time: 09/09/2021 3.30 PM
    // Last Edited By:Anjitha k k
    // Last Edited Date & Time:01-March-2022 02:30 PM
    // ======================================================== >>

    public partial class frmColorMaster : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmColorMaster(int iColorID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

                btnSave.Image = global::InventorSync.Properties.Resources.save240402;
                btnDelete.Image = global::InventorSync.Properties.Resources.delete340402;
                btnFind.Image = global::InventorSync.Properties.Resources.find_finalised_3030;
                btnMinimize.Image = global::InventorSync.Properties.Resources.minimize_finalised;
                btnClose.Image = global::InventorSync.Properties.Resources.logout_Final;

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
            txtColorName.Focus();
            txtColorName.SelectAll();
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspInsertColorMasterInfo Colorinfo = new UspInsertColorMasterInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetColorInfo GetColor = new UspGetColorInfo();
        clsColorMaster clsColor = new clsColorMaster();
        UspGetMasterInfo GetMaster = new UspGetMasterInfo();
        clsMaster clsMaster = new clsMaster();

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
            toolTipColor.SetToolTip(txtColorName, "Please specify the unique  Color");
        }
        private void txtColorHexCode_Click(object sender, EventArgs e)
        {
            toolTipColor.SetToolTip(txtColorHexCode, "Please enter the  matched the Color HexCode");
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
                txtColorName.Select();
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
                    if (txtColorName.Text != "")
                    {
                        if (txtColorName.Text != strCheck)
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
                        Comm.ControlEnterLeave(txtColorName);
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
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Color[" + txtColorName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Color [" + txtColorName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtColorName.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save...." + "\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtColorName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtColorName, true);
        }
        private void txtColorName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtColorName);
        }
        private void txtColorHexCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtColorHexCode, true);
        }
        private void txtColorHexCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtColorHexCode, false, false);
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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Color[" + txtColorName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Color [" + txtColorName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (txtColorName.Text != "")
                {
                    if (txtColorName.Text != strCheck)
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
            if (txtColorName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Color Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtColorName.Focus();
            }
            else
            {
                if (txtColorHexCode.Text == "")
                    txtColorHexCode.Text = "0";

                txtColorName.Text = txtColorName.Text.Replace("'", "\"");
            }
            return bValidate;
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetColor.ColorID = Convert.ToDecimal(iSelectedID);
            GetColor.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsColor.GetColorMaster(GetColor);
            if (dtLoad.Rows.Count > 0)
            {
                txtColorName.Text = dtLoad.Rows[0]["ColorName"].ToString();
                strCheck = dtLoad.Rows[0]["ColorName"].ToString();
                txtColorHexCode.Text = dtLoad.Rows[0]["ColorHexCode"].ToString();
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
                    Colorinfo.ColorID = Comm.gfnGetNextSerialNo("tblColor", "ColorID");
                    if (Colorinfo.ColorID < 6)
                        Colorinfo.ColorID = 6;
                }
                else
                    Colorinfo.ColorID = Convert.ToDecimal(iIDFromEditWindow);
                Colorinfo.ColorName = txtColorName.Text;
                DataTable dtUspColor = new DataTable();
                Colorinfo.ColorHexCode = txtColorHexCode.Text;
                Colorinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                strRet = clsColor.InsertUpdateDeleteColorMaster(Colorinfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Color (" + txtColorName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtColorName.Focus();
                            txtColorName.SelectAll();
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
                        CtrlPassed.Text = txtColorName.Text;
                        CtrlPassed.Tag = Colorinfo.ColorID;

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
                    Comm.MessageboxToasted("Color", "Color saved successfully");
                }
            }
        }
        //Description :  Delete Data from Color table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtUspColor = new DataTable();
            Colorinfo.ColorID = Convert.ToDecimal(iIDFromEditWindow);
            Colorinfo.ColorName = txtColorName.Text;
            Colorinfo.ColorHexCode = txtColorHexCode.Text;
            Colorinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            GetMaster.TYPE = "COLOR";
            GetMaster.ID = Convert.ToInt32(Colorinfo.ColorID);
            DataTable dtMaster = new DataTable();
            dtMaster = clsMaster.GetColumnIDsData(GetMaster);//Checking Color is Used in Item Master or not
            if (dtMaster.Rows.Count == 0)
            {
               strRet = clsColor.InsertUpdateDeleteColorMaster(Colorinfo, iAction);
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
               MessageBox.Show("Hey! There are Items Associated with this Color [" + txtColorName.Text + "]. Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtColorName.Clear();
            txtColorHexCode.Clear();
            btnDelete.Enabled = false;
            txtColorName.Focus();
        }
        #endregion
    }
}
