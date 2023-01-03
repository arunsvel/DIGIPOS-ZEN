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
    public partial class frmBrandMaster : Form, IMessageFilter
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

        // ======================================================== >>
        // Description:Brand Creation
        // Developed By:Pramod Philip
        // Completed Date & Time: 08/09/2021 3.30 PM
        // Last Edited By:Anjitha k k
        // Last Edited Date & Time:01-March-2022 12:50 PM
        // ======================================================== >>
        public frmBrandMaster(int iBrandID = 0, bool bFromEdit = false, Control Controlpassed = null)
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

            iIDFromEditWindow = iBrandID;
            bFromEditWindowBrand = bFromEdit;
            CtrlPassed = Controlpassed;
            this.BackColor = Global.gblFormBorderColor;
            if (iBrandID != 0)
            {
                LoadData(iBrandID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtBrand.Focus();
            txtBrand.SelectAll();
            Cursor.Current = Cursors.Default;
        }
        #region "VARIABLES ------------------------------------------- >>"
        UspInsertBrandMasterInfo Brandinfo = new UspInsertBrandMasterInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsBrandMaster Brandinsert = new clsBrandMaster();
        UspGetBrandinfo GetBrand = new UspGetBrandinfo();
        clsBrandMaster clsBrand = new clsBrandMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int discount = 0;
        int iAction = 0;
        Control ctrl;
        int iIDFromEditWindow;
        string strCheck = "";
        bool bFromEditWindowBrand;
        Control CtrlPassed;
        #endregion

        #region "EVENTS ---------------------------------------------- >>"
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
        private void txtBrand_Click(object sender, EventArgs e)
        {
            toolTipBrandMaster.SetToolTip(txtBrand, "Specify unique name for Brand");
        }
        private void txtBrandShortName_Click(object sender, EventArgs e)
        {
            toolTipBrandMaster.SetToolTip(txtBrandShortName, "Brand ShortName to show in print and specified area");
        }
        private void txtDiscountPerc_Click(object sender, EventArgs e)
        {
            toolTipBrandMaster.SetToolTip(txtDiscountPerc, "To calculate the Percentage of discount as per setting");
        }

        private void frmBrandMaster_Load(object sender, EventArgs e)
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
                    txtBrand.Focus();
                }
                txtBrand.Select();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  load Brand ...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmBrandMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtBrand.Text != "")
                    {
                        if (txtBrand.Text != strCheck)
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
                        if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                            txtDiscountPerc.Text = "0";
                        if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Brand") == false)
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
                    if (bFromEditWindowBrand == true)
                    {
                        try
                        {
                            if (iIDFromEditWindow > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Brand[" + txtBrand.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Brand [" + txtBrand.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Input box order is properly working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtBrand.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtBrandShortName.Focus();
            }
        }
        private void txtDiscountPerc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtBrandShortName.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    double doubleValue;
                    if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    {
                        txtDiscountPerc.Text = "0";
                    }
                    if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Brand") == false)
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
                Cursor.Current = Cursors.Default;
            }
        }
        private void txtBrand_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBrand.Text))
            {
                txtBrandShortName.Clear();
            }
        }

        //Description : Show ItemName and Item Code when write 3 letter in  Itemname Textbox
        public void ShowItemSearchDetailsinGrid(bool blnClose = false)
        {
            if (blnClose == false)
            {
                if (txtBrand.Text.Trim().Length >= 3)
                {
                    string a = txtBrand.Text;
                    string sQuery = "Select brandName,brandShortName,brandID From tblBrand where brandName LIKE '" + txtBrand.Text.Replace("'", "''").TrimStart().TrimEnd() + "%' And TenantID = '" + Global.gblTenantID + "' order by brandName";
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

        private void txtBrand_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBrand, true);
        }
        private void txtBrand_Leave(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                txtBrandShortName.Text = txtBrand.Text;

                //if (txtBrand.Text.Length > 4)
                //{
                //    if (txtBrandShortName.Text.Trim() == "")
                //        txtBrandShortName.AppendText(txtBrand.Text.Substring(0, 4));
                //}
                //else
                //{
                //    if (txtBrandShortName.Text.Trim() == "")
                //        txtBrandShortName.AppendText(txtBrand.Text);
                //}

                Comm.ControlEnterLeave(txtBrand);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  append Brand Name to Brand Shortname...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void txtBrandShortName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBrandShortName, true);
        }
        private void txtBrandShortName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBrandShortName);
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
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Brand") == true)
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
                if (iIDFromEditWindow == 0)
                {

                    if (Comm.CheckUserPermission(Common.UserActivity.new_Entry, "BRAND") == false)
                        return;

                }
                else
                {
                    if (Comm.CheckUserPermission(Common.UserActivity.UpdateEntry, "BRAND") == false)
                        return;
                }
                Cursor.Current = Cursors.WaitCursor;
                double doubleValue;
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Brand") == false)
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
                if (Comm.CheckUserPermission(Common.UserActivity.Delete_Entry, "BRAND") == false)
                    return;

                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Brand[" + txtBrand.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                      
                        DeleteData();
                        Comm.writeuserlog(Common.UserActivity.UpdateEntry, newdata, olddata, "Deleted " + Brandinfo.brandName, 0, 0, "brandName", Comm.ToInt32(Brandinfo.brandID), "Brand");

                    }
                }
                else
                    MessageBox.Show("Default Brand [" + txtBrand.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (txtBrand.Text != "")
                {
                    if (txtBrand.Text != strCheck)
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
            if (txtBrand.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Brand Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBrand.Focus();
            }
            //else if (txtBrandShortName.Text.Trim() == "")
            //{
            //    bValidate = false;
            //    MessageBox.Show("Please enter Brand Short Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    txtBrandShortName.Focus();
            //}
            else
            {
                if (txtDiscountPerc.Text == "")
                    txtDiscountPerc.Text = "0";
                txtBrand.Text = txtBrand.Text.Replace("'", "\"");
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
            DataTable dtLoad = new DataTable();
            GetBrand.brandID = Convert.ToDecimal(iSelectedID);
            GetBrand.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsBrand.GetBrandMaster(GetBrand);
            if (dtLoad.Rows.Count > 0)
            {
                txtBrand.Text = dtLoad.Rows[0]["BrandName"].ToString();
                strCheck = dtLoad.Rows[0]["BrandName"].ToString();
                txtBrandShortName.Text = dtLoad.Rows[0]["BrandShortName"].ToString();
                decimal DiscPer = Convert.ToDecimal(dtLoad.Rows[0]["DiscPer"].ToString());
                txtDiscountPerc.Text = FormatValue(Convert.ToDouble(DiscPer), true, "#.00");
                iAction = 1;
            }
            olddata = "BrandName:" + txtBrand.Text + ",BrandShortName:" + txtBrandShortName + ",Discount%:" + txtDiscountPerc.Text;
            oldvalue = txtBrand.Text;

        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                newdata = "BrandName:" + txtBrand.Text + ",BrandShortName:" + txtBrandShortName + ",Discount%:" + txtDiscountPerc.Text;

                string[] strResult;
                string strRet = "";
                if (iAction == 0)
                {
                    Brandinfo.brandID = Comm.gfnGetNextSerialNo("tblBrand", "brandID");
                    if (Brandinfo.brandID < 6)
                        Brandinfo.brandID = 6;
                }
                else
                    Brandinfo.brandID = iIDFromEditWindow;
                DataTable dtUspBrand = new DataTable();
                Brandinfo.brandName = txtBrand.Text;
                if (txtBrandShortName.Text.Trim() == "")
                {
                    //if (txtBrand.Text.Length > 4)
                    //    txtBrandShortName.Text = txtBrand.Text.Substring(0, 4);
                    //else
                        txtBrandShortName.Text = txtBrand.Text;
                }
                    Brandinfo.brandShortName = txtBrandShortName.Text;
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    Brandinfo.DiscPer = 0;
                else
                    Brandinfo.DiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
                Brandinfo.SystemName = Environment.MachineName;
                Brandinfo.UserID = Convert.ToDecimal(Global.gblUserID);
                Brandinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                Brandinfo.LastUpdateDate = DateTime.Today;
                Brandinfo.LastUpdateTime = DateTime.Now;
                strRet = Brandinsert.InsertUpdateDeleteBrandMaster(Brandinfo, iAction);
                if (strRet.Length > 2)
                {
                   strResult = strRet.Split('|');
                   if (Convert.ToInt32(strResult[0].ToString()) == -1)
                   {
                       if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                       {
                          if (strResult[1].ToString().Contains("UK_BrandShortName"))
                          {
                              MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Brand short name (" + txtBrandShortName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                              txtBrandShortName.Focus();
                              txtBrandShortName.SelectAll();
                          }
                          else
                          {
                              MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Brand name(" + txtBrand.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                              txtBrand.Focus();
                              txtBrand.SelectAll();
                          }
                       }
                       else
                           MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            CtrlPassed.Text = txtBrandShortName.Text;
                            CtrlPassed.Tag = Brandinfo.brandID;
                            CtrlPassed.Focus();
                            this.Close();
                        }
                        else if (CtrlPassed is ComboBox)
                        {
                            DataTable dtBrand = Comm.fnGetData("SELECT brandID,brandShortName FROM tblBrand WHERE TenantID = " + Global.gblTenantID + " ORDER BY brandShortName Asc").Tables[0];
                            ((ComboBox)CtrlPassed).DataSource = dtBrand;
                            ((ComboBox)CtrlPassed).DisplayMember = "brandShortName";
                            ((ComboBox)CtrlPassed).ValueMember = "brandID";
                            ((ComboBox)CtrlPassed).SelectedValue = Brandinfo.brandID;
                            ((ComboBox)CtrlPassed).Tag = Brandinfo.brandID;

                            CtrlPassed.Focus();
                            this.Close();
                        }

                    }
                    else
                       ClearAll();
                   if (bFromEditWindowBrand == true)
                   {
                      this.Close();
                   }
                   Comm.MessageboxToasted("Brand", "Brand saved successfully");
                    if (iIDFromEditWindow > 0)
                    {

                        Comm.writeuserlog(Common.UserActivity.UpdateEntry, newdata, olddata, "Update " + oldvalue + " Brand to " + Brandinfo.brandName, 0, 0, "brandName", Comm.ToInt32(Brandinfo.brandID), "Brand");

                    }
                    else
                    {

                        Comm.writeuserlog(Common.UserActivity.new_Entry, newdata, olddata, "Created " + Brandinfo.brandName, 0, 0, "brandName", Comm.ToInt32(Brandinfo.brandID), "Brand");

                    }
                }
            }
        }
        //Description :  Delete Data from Brand table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtUspBrand = new DataTable();
            Brandinfo.brandID = iIDFromEditWindow;
            Brandinfo.brandName = txtBrand.Text;
            Brandinfo.brandShortName = txtBrandShortName.Text;
            Brandinfo.DiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
            Brandinfo.SystemName = Global.gblSystemName;
            Brandinfo.UserID = Global.gblUserID;
            Brandinfo.TenantID = Global.gblTenantID;
            Brandinfo.LastUpdateDate = DateTime.Today;
            Brandinfo.LastUpdateTime = DateTime.Now;
            strRet = clsBrand.InsertUpdateDeleteBrandMaster(Brandinfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are Items Associated with this Brand [" + txtBrand.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (bFromEditWindowBrand == true)
            {
                this.Close();
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtBrand.Clear();
            txtBrandShortName.Clear();
            btnDelete.Enabled = false;
            txtDiscountPerc.Text = "0";
            txtBrand.Focus();
        }
        #endregion

        private void txtBrand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (iIDFromEditWindow == 0)
                ShowItemSearchDetailsinGrid();
        }
    }
}








