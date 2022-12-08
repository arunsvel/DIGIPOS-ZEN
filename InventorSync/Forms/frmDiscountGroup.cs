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
    public partial class frmDiscountGroup : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description:Discount Group Creation
        // Developed By:Pramod Philip
        // Completed Date & Time:10/09/2021 3.00 PM
        // Last Edited By:Anjitha k k
        // Last Edited Date & Time:01-March-2022 1:10 PM
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmDiscountGroup(int iDiscountID = 0, bool bFromEdit = false,Control Controlpassed = null)
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

            iIDFromEditWindow = iDiscountID;
            bFromEditWindowDisc = bFromEdit;
            CtrlPassed = Controlpassed;
            this.BackColor = Global.gblFormBorderColor;
            if (iDiscountID != 0)
            {
                LoadData(iDiscountID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtDiscountGroupName.Focus();
            txtDiscountGroupName.SelectAll();
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspInsertDiscountGroupInfo DiscounGtroupinfo = new UspInsertDiscountGroupInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsDiscountGroup DiscountGroupinsert = new clsDiscountGroup();
        UspGetDiscountGroupInfo GetDiscountGroup = new UspGetDiscountGroupInfo();
        clsDiscountGroup clsDiscountGroup = new clsDiscountGroup();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int discount = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        Control ctrl;
        string strCheck;
        bool bFromEditWindowDisc;
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
        private void txtDiscountGroupName_Click(object sender, EventArgs e)
        {
            toolTipGroupMaster.SetToolTip(txtDiscountGroupName, "Specify unique name for Discount Group");
        }
        private void txtDiscountPerc_Click(object sender, EventArgs e)
        {
            toolTipGroupMaster.SetToolTip(txtDiscountPerc, "To calculate the Percentage of discount as per setting");
        }

        private void frmDiscountGroup_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {

                    ClearAll();
                    txtDiscountPerc.Text = discount.ToString();
                    this.Show();
                    Application.DoEvents();
                    txtDiscountGroupName.Focus();
                }
                txtDiscountGroupName.Select();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Discount Group  ...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmDiscountGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtDiscountGroupName.Text != "")
                    {
                        if (txtDiscountGroupName.Text != strCheck)
                        {
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
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
                        if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                            txtDiscountPerc.Text = "0";
                        if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Discount Group") == false)
                        {
                            Comm.ControlEnterLeave(txtDiscountPerc, false, false);
                            txtDiscountPerc.Text = FormatValue(Convert.ToDouble(txtDiscountPerc.Text), true, "#.00");
                            SaveData();
                        }
                        else
                        {
                            txtDiscountPerc.Text = "99";
                            txtDiscountPerc.Focus();
                            txtDiscountPerc.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowDisc == true)
                    {
                        try
                        {
                            if (txtDiscountGroupName.Text == "")
                            {
                                btnDelete.Enabled = false;
                            }
                            else
                            {
                                if (iIDFromEditWindow > 5)
                                {
                                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Discount Group[" + txtDiscountGroupName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                    if (dlgResult == DialogResult.Yes)
                                    {
                                        DeleteData();
                                    }
                                }
                                else
                                    MessageBox.Show("Default Discount Group [" + txtDiscountGroupName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to Delete ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
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
                MessageBox.Show("Shortcut Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtDiscountGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
            {
                this.SelectNextControl(ctrl, false, false, false, false);
            }
            else if(e.KeyCode==Keys.Enter)
                txtDiscountPerc.Focus();
        }
        private void txtDiscountPerc_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
            {
                this.SelectNextControl(ctrl, false, false, false, false);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                        txtDiscountPerc.Text = "0";
                    if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Discount Group") == false)
                    {
                        Comm.ControlEnterLeave(txtDiscountPerc, false, false);
                        txtDiscountPerc.Text = FormatValue(Convert.ToDouble(txtDiscountPerc.Text), true, "#.00");
                        SaveData();
                    }
                    else
                    {
                         txtDiscountPerc.Text = "99";
                         txtDiscountPerc.Focus();
                         txtDiscountPerc.SelectAll();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Save...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtDiscountPerc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                           (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Numeric input Field not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtDiscountGroupName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDiscountGroupName, true);
        }
        private void txtDiscountGroupName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDiscountGroupName);
        }
        private void txtDiscountPerc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDiscountPerc, true);
        }
        private void txtDiscountPerc_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                else if (txtDiscountPerc.Text.TrimEnd().TrimStart() == ".")
                    txtDiscountPerc.Text = ".0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Discount Group") == true)
                {
                    txtDiscountPerc.Text = "99";
                    txtDiscountPerc.Focus();
                    txtDiscountPerc.SelectAll();
                }
                Comm.ControlEnterLeave(txtDiscountPerc, false, false);
                txtDiscountPerc.Text = FormatValue(Convert.ToDouble(txtDiscountPerc.Text), true, "#.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Limit of Discount Percentage...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                {
                    if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                        txtDiscountPerc.Text = "0";
                    if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Discount Group") == false)
                    {
                        Comm.ControlEnterLeave(txtDiscountPerc, false, false);
                        txtDiscountPerc.Text = FormatValue(Convert.ToDouble(txtDiscountPerc.Text), true, "#.00");
                        SaveData();
                    }
                    else
                    {
                        txtDiscountPerc.Text = "99";
                        txtDiscountPerc.Focus();
                        txtDiscountPerc.SelectAll();
                    }
                }
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
                if (iIDFromEditWindow > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete  from  Discount Group  [" + txtDiscountGroupName.Text + "]   Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Discount Group [" + txtDiscountGroupName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            Cursor.Current = Cursors.Default;
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
                if (txtDiscountGroupName.Text != "")
                {
                if (txtDiscountGroupName.Text != strCheck)
                {
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
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
            if (txtDiscountGroupName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Discount Group Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDiscountGroupName.Focus();
            }
            else
            {
                if (txtDiscountPerc.Text == "")
                    txtDiscountPerc.Text = "0";
                txtDiscountGroupName.Text = txtDiscountGroupName.Text.Replace("'", "\"");
            }
            return bValidate;
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
                myFormat = "#.00";
            if (sMyFormat != "")
                myFormat = sMyFormat;
            return Convert.ToDouble(myValue).ToString(myFormat);
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            Cursor.Current = Cursors.WaitCursor;
            DataTable dtLoad = new DataTable();
            GetDiscountGroup.DiscountGroupID = Convert.ToDecimal(iSelectedID);
            GetDiscountGroup.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsDiscountGroup.GetDiscountGroup(GetDiscountGroup);
            if (dtLoad.Rows.Count > 0)
            {
                txtDiscountGroupName.Text = dtLoad.Rows[0]["DiscountGroupName"].ToString();
                strCheck = dtLoad.Rows[0]["DiscountGroupName"].ToString();
                decimal DiscPer = Convert.ToDecimal(dtLoad.Rows[0]["DiscPer"].ToString());
                txtDiscountPerc.Text = FormatValue(Convert.ToDouble(DiscPer), true, "#.00");
                iAction = 1;
            }
            Cursor.Current = Cursors.Default;
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            Cursor.Current = Cursors.WaitCursor;
            if (IsValidate() == true)
            {
                string[] strResult;
                string strRet = "";
            
                DataTable dtUspDiscountGroup = new DataTable();
                if (iAction == 0)
                {
                    DiscounGtroupinfo.DiscountGroupID = Comm.gfnGetNextSerialNo("tblDiscountGroup", "DiscountGroupID");
                    if (DiscounGtroupinfo.DiscountGroupID < 6)
                        DiscounGtroupinfo.DiscountGroupID = 6;
                }
                else
                    DiscounGtroupinfo.DiscountGroupID = iIDFromEditWindow;
                DiscounGtroupinfo.DiscountGroupName = txtDiscountGroupName.Text;
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                else
                    DiscounGtroupinfo.DiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
                DiscounGtroupinfo.SystemName = Environment.MachineName;
                DiscounGtroupinfo.UserID = Convert.ToDecimal(Global.gblUserID);
                DiscounGtroupinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                DiscounGtroupinfo.LastUpdateDate = DateTime.Today;
                DiscounGtroupinfo.LastUpdateTime = DateTime.Now;
                strRet = clsDiscountGroup.InsertUpdateDeleteDiscountGroup(DiscounGtroupinfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the Discount Group name(" + txtDiscountGroupName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtDiscountGroupName.Focus();
                            txtDiscountGroupName.SelectAll();
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        if (CtrlPassed is TextBox)
                        {
                            CtrlPassed.Text = txtDiscountGroupName.Text;
                            CtrlPassed.Tag = DiscounGtroupinfo.DiscountGroupID;
                            CtrlPassed.Focus();
                            this.Close();
                        }
                        else if (CtrlPassed is ComboBox)
                        {
                            DataTable dtBrand = Comm.fnGetData("SELECT DiscountGroupID,DiscountGroupName FROM tblDiscountGroup  WHERE TenantID = " + Global.gblTenantID + " ORDER BY DiscountGroupName Asc").Tables[0];
                            ((ComboBox)CtrlPassed).DataSource = dtBrand;
                            ((ComboBox)CtrlPassed).DisplayMember = "DiscountGroupName";
                            ((ComboBox)CtrlPassed).ValueMember = "DiscountGroupID";
                            ((ComboBox)CtrlPassed).SelectedValue = DiscounGtroupinfo.DiscountGroupID;
                            ((ComboBox)CtrlPassed).Tag = DiscounGtroupinfo.DiscountGroupID;

                            CtrlPassed.Focus();
                            this.Close();
                        }
                    }
                    else
                    {
                        ClearAll();
                        txtDiscountPerc.Text = discount.ToString();
                        if (bFromEditWindowDisc == true)
                        {
                            this.Close();
                        }
                    }
                    Comm.MessageboxToasted("Discount Group", "Discount Group saved successfully");
                }
            }
            Cursor.Current = Cursors.Default;
        }
        //Description :  Delete Data from Discount Group table
        private void DeleteData()
        {
            Cursor.Current = Cursors.WaitCursor;
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtUspUnit = new DataTable();
            DiscounGtroupinfo.DiscountGroupID = iIDFromEditWindow;
            DiscounGtroupinfo.DiscountGroupName = txtDiscountGroupName.Text;
            DiscounGtroupinfo.DiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
            DiscounGtroupinfo.SystemName = Global.gblSystemName;
            DiscounGtroupinfo.UserID = Global.gblUserID;
            DiscounGtroupinfo.TenantID = Global.gblTenantID;
            DiscounGtroupinfo.LastUpdateDate = DateTime.Today;
            DiscounGtroupinfo.LastUpdateTime = DateTime.Now;
            strRet = DiscountGroupinsert.InsertUpdateDeleteDiscountGroup(DiscounGtroupinfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are Items Associated with this Discount Group [" + txtDiscountGroupName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (Convert.ToInt32(strRet) == -1)
                    MessageBox.Show("Failed to Delete ?", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    ClearAll();
            }
            if (bFromEditWindowDisc == true)
            {
                this.Close();
            }
            Cursor.Current = Cursors.Default;
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtDiscountGroupName.Clear();
            txtDiscountGroupName.Clear();
            btnDelete.Enabled = false;
            txtDiscountPerc.Text = "0";
            txtDiscountGroupName.Focus();
        }
        #endregion

        private void txtDiscountGroupName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (iIDFromEditWindow == 0)
                ShowItemSearchDetailsinGrid();

        }
        //Description : Show ItemName and Item Code when write 3 letter in  Itemname Textbox
        public void ShowItemSearchDetailsinGrid(bool blnClose = false)
        {
            if (blnClose == false)
            {
                if (txtDiscountGroupName.Text.Trim().Length >= 3)
                {
                    string a = txtDiscountGroupName.Text;
                    string sQuery = "Select DiscountGroupName,DiscPer,DiscountGroupID From tblDiscountGroup where brandName LIKE '" + txtDiscountGroupName.Text.Replace("'", "''").TrimStart().TrimEnd() + "%' And TenantID = '" + Global.gblTenantID + "' order by DiscountGroupName";
                    DataTable dtItemExist = Comm.fnGetData(sQuery).Tables[0];
                    if (dtItemExist.Rows.Count > 0)
                    {
                        pnlShowSearch.Visible = true;

                        dgvShowItemSearch.AutoGenerateColumns = false;
                        dgvShowItemSearch.DataSource = dtItemExist;
                    }
                }
            }
        }
    }
}

