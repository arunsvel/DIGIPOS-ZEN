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

    // ======================================================== >>
    // Description:Size Master Creation
    // Developed By:Pramod Philip
    // Completed Date & Time: 09/09/2021 6.00 PM
    // Last Edited By:Anjitha
    // Last Edited Date & Time:01-March-2022 02:00 PM
    // ======================================================== >>
    public partial class FrmSizeMaster : Form, IMessageFilter
    {
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
        public FrmSizeMaster(int iSizeID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

            iIDFromEditWindow = iSizeID;
            bFromEditWindowSize = bFromEdit;
            CtrlPassed = Controlpassed;
            this.BackColor = Global.gblFormBorderColor;
            if (iSizeID != 0)
            {
                LoadData(iSizeID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtSizeName.Focus();
            txtSizeName.SelectAll();
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspInsertSizeMasterInfo sizeinfo = new UspInsertSizeMasterInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetSizeInfo GetSize = new UspGetSizeInfo();
        clsSizeMaster clsSize = new clsSizeMaster();
        UspGetMasterInfo GetMaster = new UspGetMasterInfo();
        clsMaster clsMaster = new clsMaster();
      
        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int discount = 1;
        int iAction = 0;
        int iIDFromEditWindow;
        string strCheck;
        Control ctrl;
        bool bFromEditWindowSize;
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
        private void txtSizeName_Click(object sender, EventArgs e)
        {
            toolTipSizeMaster.SetToolTip(txtSizeName, "Please specify unique name for Size");
        }
        private void txtSizeShortName_Click(object sender, EventArgs e)
        {
            toolTipSizeMaster.SetToolTip(txtSizeShortName, "Size ShortName to show in print and specified area");
        }
        private void txtSortOrder_Click(object sender, EventArgs e)
        {
            toolTipSizeMaster.SetToolTip(txtSortOrder, "Please enter  the  Sort Order of Size");
        }

        private void FrmSizeMaster_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();
                    this.Show();
                    Application.DoEvents();
                    FillSortOrder();
                }
                txtSizeName.Select();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  load Size ...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }                
        private void FrmSizeMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtSizeName.Text != "")
                    {
                        if (txtSizeName.Text != strCheck)
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
                        Cursor.Current = Cursors.WaitCursor;

                        Comm.ControlEnterLeave(txtSizeName);
                        Comm.ControlEnterLeave(txtSizeShortName);
                        Application.DoEvents();

                        SaveData();

                        Cursor.Current = Cursors.Default;
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowSize == true)
                    {
                        try
                        {
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Size[" + txtSizeName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Size [" + txtSizeName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly"+"\n"+ ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void txtSizeName_KeyDown(object sender, KeyEventArgs e)
        {
            ctrl = (Control)sender;
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
        private void txtSortOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtSizeShortName.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save...." + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtSortOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show("Sort Order numeric key entry Failed .." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtSizeName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSizeName.Text))
            {
                txtSizeShortName.Clear();
            }
        }
        private void txtSizeName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSizeName, true);
        }
        private void txtSizeName_Leave(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (txtSizeName.Text.Length > 4)
                {
                    if (txtSizeShortName.Text.Trim() == "")
                        txtSizeShortName.Text = txtSizeName.Text;
                        //txtSizeShortName.AppendText(txtSizeName.Text.Substring(0, 4));
                }
                else
                {
                    if (txtSizeShortName.Text.Trim() == "")
                        txtSizeShortName.Text = txtSizeName.Text;
                        //txtSizeShortName.AppendText(txtSizeName.Text);
                }
                Comm.ControlEnterLeave(txtSizeName);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  append Size Name to Size Shortname...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtSizeShortName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSizeShortName, true);
        }
        private void txtSizeShortName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSizeShortName);
        }
        private void txtSortOrder_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, true);
        }
        private void txtSortOrder_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, false, false);
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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Size[" + txtSizeName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                        Comm.writeuserlog(Common.UserActivity.Delete_Entry, newdata, olddata, "Deleted " + sizeinfo.SizeName, 518, 518, sizeinfo.SizeName, Comm.ToInt32(sizeinfo.SizeID), "Size");

                    }
                }
                else
                    MessageBox.Show("Default Size [" + txtSizeName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (txtSizeName.Text != "")
                {
                    if (txtSizeName.Text != strCheck)
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
            if (txtSizeName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Size Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSizeName.Focus();
            }
            //else if (txtSizeShortName.Text.Trim() == "")
            //{
            //    bValidate = false;
            //    MessageBox.Show("Please enter Size Short Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    txtSizeShortName.Focus();
            //}
            else
            {
                if (txtSortOrder.Text == "")
                    txtSortOrder.Text = "0";
                txtSizeName.Text = txtSizeName.Text.Replace("'", "\"");
            }
            return bValidate;
        }
        //Description : Fill For Sort Order 
        private void FillSortOrder()
        {
            DataTable dtsortOrder = new DataTable();
            dtsortOrder = Comm.fnGetData("SELECT MAX(ISNULL(SortOrder, 0)) + 1 as SortOrder FROM tblSize WHERE TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtsortOrder.Rows.Count > 0)
            {
                txtSortOrder.Text = dtsortOrder.Rows[0]["SortOrder"].ToString();
            }
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataTable dtLoad = new DataTable();
                GetSize.SizeID = Convert.ToDecimal(iSelectedID);
                GetSize.TenantID = Convert.ToDecimal(Global.gblTenantID);
                dtLoad = clsSize.GetSizeMaster(GetSize);
                if (dtLoad.Rows.Count > 0)
                {
                    txtSizeName.Text = dtLoad.Rows[0]["SizeName"].ToString();
                    strCheck = dtLoad.Rows[0]["SizeName"].ToString();
                    txtSizeShortName.Text = dtLoad.Rows[0]["SizeNameShort"].ToString();
                    txtSortOrder.Text = dtLoad.Rows[0]["SortOrder"].ToString();
                    iAction = 1;
                }
                oldvalue = txtSizeName.Text;
                olddata = "Size Name:" + txtSizeName.Text + ",SizeNameShort:" + txtSizeShortName.Text + ",SortOrder:" + txtSortOrder.Text;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed To Load..."+"\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                newdata = "Size Name:" + txtSizeName.Text + ",SizeNameShort:" + txtSizeShortName.Text + ",SortOrder:" + txtSortOrder.Text;

                string[] strResult;
                string strRet = "";
                if (iAction == 0)
                {
                    sizeinfo.SizeID = Comm.gfnGetNextSerialNo("tblSize", "SizeID");
                    if (sizeinfo.SizeID < 6)
                        sizeinfo.SizeID = 6;
                }
                else
                    sizeinfo.SizeID = Convert.ToDecimal(iIDFromEditWindow);
                sizeinfo.SizeName = txtSizeName.Text;
                DataTable dtUspSize = new DataTable();
                if (txtSizeShortName.Text.Trim() == "")
                {
                    if (txtSizeName.Text.Length > 4)
                        txtSizeShortName.Text = txtSizeName.Text.Substring(0, 4);
                    else
                        txtSizeShortName.Text = txtSizeName.Text;
                }
                sizeinfo.SizeNameShort = txtSizeShortName.Text;
                sizeinfo.SortOrder = Convert.ToDecimal(txtSortOrder.Text);           
                sizeinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                strRet = clsSize.InsertUpdateDeleteSizeMaster(sizeinfo, iAction);
                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                if (strResult[1].ToString().Contains("UK_SizeShortName"))
                                {
                                    MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Size short name (" + txtSizeShortName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    txtSizeShortName.Focus();
                                    txtSizeShortName.SelectAll();
                                }
                                else
                                {
                                    MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Size name(" + txtSizeName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    txtSizeName.Focus();
                                    txtSizeName.SelectAll();
                                }
                            }
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Save! ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                        {
                             CtrlPassed.Text = txtSizeName.Text;
                             CtrlPassed.Tag = sizeinfo.SizeID;

                             CtrlPassed.Focus();
                             this.Close();
                        }
                        else
                        {
                            ClearAll();
                            if (bFromEditWindowSize == true)
                            {
                                this.Close();
                            }
                        }
                        Comm.MessageboxToasted("Size", "Size saved successfully");
                    if (iIDFromEditWindow > 0)
                    {

                        Comm.writeuserlog(Common.UserActivity.UpdateEntry, newdata, olddata, "Update " + oldvalue + " Size to " + sizeinfo.SizeName, 518, 518, sizeinfo.SizeName, Comm.ToInt32(sizeinfo.SizeID), "Size");

                    }
                    else
                    {

                        Comm.writeuserlog(Common.UserActivity.new_Entry, newdata, olddata, "Created " + sizeinfo.SizeName, 518, 518, sizeinfo.SizeName, Comm.ToInt32(sizeinfo.SizeID), "Size");

                    }
                }
            }
         }
        //Description :  Delete Data from Size table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtUspSize = new DataTable();
            sizeinfo.SizeID = Convert.ToDecimal(iIDFromEditWindow);
            sizeinfo.SizeName = txtSizeName.Text;
            sizeinfo.SizeNameShort = txtSizeShortName.Text;
            sizeinfo.SortOrder = Convert.ToDecimal(txtSortOrder.Text);
            sizeinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            GetMaster.TYPE = "SIZE";
            GetMaster.ID = Convert.ToInt32(sizeinfo.SizeID);
            DataTable dtMaster = new DataTable();
            dtMaster = clsMaster.GetColumnIDsData(GetMaster);//Checking Size is used in Item Master or not
            if(dtMaster.Rows.Count==0)
            {
               strRet = clsSize.InsertUpdateDeleteSizeMaster(sizeinfo, iAction);
               if (strRet.Length > 2)
               {
                  strResult = strRet.Split('|');
                  if (Convert.ToInt32(strResult[0].ToString()) == -1)
                     MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
               }
               else
               {
                  if (Convert.ToInt32(strRet) == -1)
                      MessageBox.Show("Failed to Delete", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                  else
                    ClearAll();
               }
               if (bFromEditWindowSize == true)
               {
                   this.Close();
               }
             }
             else
                 MessageBox.Show("Hey! There are Items Associated with this Size [" + txtSizeName.Text + "]. Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtSizeName.Clear();
            txtSizeShortName.Clear();
            txtSortOrder.Clear();
            btnDelete.Enabled = false;
            FillSortOrder();
            txtSizeName.Focus();
        }
        #endregion
    }
}

