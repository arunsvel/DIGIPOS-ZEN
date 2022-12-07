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
    public partial class frmManufacturer : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description: The Manufacturer of the Item
        // Developed By: Dipu Joseph
        // Completed Date & Time: 06-Sep-2021 7.00 PM
        // Last Edited By: Anjitha
        // Last Edited Date & Time:01-March-2022 11:38 AM
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmManufacturer(int iManfID = 0, bool bFromEdit = false,Control Controlpassed = null, bool blnDisableMinimize = false)
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

                iIDFromEditWindow = iManfID;
                bFromEditWindow = bFromEdit;
                CtrlPassed = Controlpassed;


                lblMand1.Location = new Point(158, 17);
                this.BackColor = Global.gblFormBorderColor;
                if (iManfID != 0)
                {
                    LoadData(iManfID);
                }
                else
                {
                    btnDelete.Enabled = false;
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtManufacture.Text = CtrlPassed.Text.ToString();
                }

                txtManufacture.Focus();
                txtManufacture.SelectAll();
                txtManufacture.SelectionStart = txtManufacture.Text.ToString().Length;

                if (blnDisableMinimize == true) btnMinimize.Enabled = false;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspManufacturerInsertInfo ManfInfo = new UspManufacturerInsertInfo();
        UspGetManufacturerInfo GetManf = new UspGetManufacturerInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsManufacturer clsManf = new clsManufacturer();
        clsTheme Theme = new clsTheme();

        //For Drag Form
        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;

        int iAction = 0;
        int iIDFromEditWindow;
        string strCheck="";
        Control ctrl;
        bool bFromEditWindow;
        Control CtrlPassed;
        #endregion

        #region "EVENTS ----------------------------------------------- >>"
        //For Drag Form
        private void tlpHeader_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                dragging = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void tlpHeader_MouseDown(object sender, MouseEventArgs e)
        {
            try
            { 
                dragging = true;
                xOffset = Cursor.Position.X - this.Location.X;
                yOffset = Cursor.Position.Y - this.Location.Y;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void tlpHeader_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (dragging)
                {
                    this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                    this.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //For Help Using ToolTip
        private void txtManufacture_Click(object sender, EventArgs e)
        {
            toolTipManufacturer.SetToolTip(txtManufacture, "Specify unique name for manufacturer");
        }
        private void txtManfShortName_Click(object sender, EventArgs e)
        {
            toolTipManufacturer.SetToolTip(txtManfShortName, "Manufacturer ShortName to show in print and specified area");
        }
        private void txtDiscountPerc_Click(object sender, EventArgs e)
        {
            toolTipManufacturer.SetToolTip(txtDiscountPerc, "To calculate the Percentage of discount as per setting");
        }

        private void frmManufacture_Load(object sender, EventArgs e)
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

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtManufacture.Text = CtrlPassed.Text.ToString();
                }

                txtManufacture.Select();
                txtManufacture.SelectionStart = txtManufacture.Text.ToString().Length;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  load Manufacture ...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmManufacturer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtManufacture.Text != "")
                    {
                        if (txtManufacture.Text != strCheck)
                        {
                            DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                            if (dlgResult.Equals(DialogResult.Yes))
                            {
                                this.Close();
                            }
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
                //    //frmEdit.MdiParent = this.MdiParent;
                //    frmEdit.Show();
                //}
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                        if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                            txtDiscountPerc.Text = "0";
                        if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Manufacturer") == false)
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
                    if (bFromEditWindow == true)
                    {
                        try
                        {
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Manufacture[" + txtManufacture.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Manufacture [" + txtManufacture.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to Delete" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Shortcut keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        #region "Move focus automatically when Enter ------------------ >>"
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
                MessageBox.Show("Shortcut keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        #endregion
        private void txtManufacture_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtManufacture.Focus();
                txtManufacture.SelectionStart = txtManufacture.Text.ToString().Length;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtManfShortName.Focus();
            }
        }
        private void txtDiscountPerc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtManfShortName.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    double doubleValue;
                    if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                        txtDiscountPerc.Text = "0";
                    if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Manufacturer") == false)
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
        private void txtDiscountPerc_KeyPress(object sender, KeyPressEventArgs e)//Set Numeric Values
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
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
                MessageBox.Show("Failed to numeric key values entries...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtManufacture_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtManufacture.Text))
                {
                    txtManfShortName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtManufacture_Enter(object sender, EventArgs e)
        {
            try
            {
                Comm.ControlEnterLeave(txtManufacture, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtManufacture_Leave(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (txtManufacture.Text.Length > 4)
                {
                    if (txtManfShortName.Text.Trim() == "")
                        txtManfShortName.Text = txtManufacture.Text;
                        //txtManfShortName.Text = txtManufacture.Text.Substring(0, 4);
                }
                else
                {
                    if (txtManfShortName.Text.Trim() == "")
                        txtManfShortName.Text = txtManufacture.Text;
                }
                Comm.ControlEnterLeave(txtManufacture);//For Casing
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to append Manufacture name to  Manufacturer shortname..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtManfShortName_Enter(object sender, EventArgs e)
        {
            try
            {
                Comm.ControlEnterLeave(txtManfShortName, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtManfShortName_Leave(object sender, EventArgs e)
        {
            try
            {
                Comm.ControlEnterLeave(txtManfShortName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtDiscountPerc_Enter(object sender, EventArgs e)
        {
            try
            {
                Comm.ControlEnterLeave(txtDiscountPerc, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtDiscountPerc_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                else if (txtDiscountPerc.Text.TrimEnd().TrimStart() == ".")
                    txtDiscountPerc.Text = ".0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Manufacturer") == true)
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
                double doubleValue;

                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Manufacturer") == false)
                {
                    Comm.ControlEnterLeave(txtManufacture);
                    Application.DoEvents();

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
            Cursor.Current = Cursors.Default;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Manufacturer[" + txtManufacture.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Manufacture [" + txtManufacture.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (txtManufacture.Text != "")
                {
                     if (txtManufacture.Text != strCheck)
                     {
                        DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult.Equals(DialogResult.Yes))
                         {
                             this.Close();
                         }
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

        #region "METHODS ---------------------------------------------- >>"
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
            if (txtManufacture.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Manufacturer Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtManufacture.Focus();
                txtManufacture.SelectionStart = txtManufacture.Text.ToString().Length;
            }
            //else if (txtManfShortName.Text.Trim() == "")
            //{
            //    bValidate = false;
            //    MessageBox.Show("Please enter Manufacturer Short Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    txtManfShortName.Focus();
            //}
            else
            {
                if (txtDiscountPerc.Text == "")
                    txtDiscountPerc.Text = "0";
                    txtManufacture.Text = txtManufacture.Text.Replace("'", "\"");
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
            try
            {
                DataTable dtLoad = new DataTable();
                GetManf.MnfID = Convert.ToDecimal(iSelectedID);
                GetManf.TenantID = Convert.ToDecimal(Global.gblTenantID);
                dtLoad = clsManf.GetManufacturer(GetManf);
                if (dtLoad.Rows.Count > 0)
                {
                    txtManufacture.Text = dtLoad.Rows[0]["MnfName"].ToString();
                    strCheck = dtLoad.Rows[0]["MnfName"].ToString();
                    txtManfShortName.Text = dtLoad.Rows[0]["MnfShortName"].ToString();
                    decimal DiscPer = Comm.ToDecimal(dtLoad.Rows[0]["DiscPer"].ToString());
                    txtDiscountPerc.Text = FormatValue(Comm.ToDouble(DiscPer), true, "#.00");
                    iAction = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Manufacturer is loading  not properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            try 
            {
                if (IsValidate() == true)
                {
                    string[] strResult;
                    string sRet = "";
                    DataTable dtUspMa = new DataTable();
                    if (iAction == 0)
                    {
                        ManfInfo.MnfID = Convert.ToDecimal(Comm.gfnGetNextSerialNo("tblManufacturer", "MnfID"));
                        if (ManfInfo.MnfID < 6)
                            ManfInfo.MnfID = 6;
                    }
                    else
                        ManfInfo.MnfID = Convert.ToDecimal(iIDFromEditWindow);
                    ManfInfo.MnfName = txtManufacture.Text;
                    if (txtManfShortName.Text.Trim() == "")
                    {
                        if (txtManufacture.Text.Length > 4)
                            txtManfShortName.Text = txtManufacture.Text.Substring(0, 4);
                        else
                            txtManfShortName.Text = txtManufacture.Text;
                    }
                    ManfInfo.MnfShortName = txtManfShortName.Text;
                    if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                        txtDiscountPerc.Text = "0";
                    ManfInfo.DiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
                    ManfInfo.SystemName = Global.gblSystemName;
                    ManfInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                    ManfInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                    ManfInfo.LastUpdateDate = DateTime.Today;
                    ManfInfo.LastUpdateTime = DateTime.Now;
                    sRet = clsManf.InsertUpdateDeleteManufacturer(ManfInfo, iAction);
                    if (sRet.Length > 2)
                    {
                       strResult = sRet.Split('|');
                       if (Convert.ToInt32(strResult[0].ToString()) == -1)
                       {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                if (strResult[1].ToString().Contains("UK_ManufacturerShortName"))
                                {
                                    MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Manufacture short name (" + txtManfShortName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    txtManfShortName.Focus();
                                    txtManfShortName.SelectAll();
                                }
                                else
                                {
                                    MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Manufacture name(" + txtManufacture.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    txtManufacture.Focus();
                                    txtManufacture.SelectAll();
                                    txtManufacture.SelectionStart = txtManufacture.Text.ToString().Length;
                                }
                            }
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                       }
                    }
                    else
                    {
                        if (Convert.ToInt32(sRet) == -1)
                            MessageBox.Show("Failed to Save ...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                        {
                             CtrlPassed.Text = txtManufacture.Text;
                             CtrlPassed.Tag = ManfInfo.MnfID;
                             CtrlPassed.Focus();
                            this.Close();
                        }
                        else
                        {
                            if (bFromEditWindow == true)
                                this.Close();
                            ClearAll();
                        }
                        Comm.MessageboxToasted("Manufacture", "Manufacture saved successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        //Description :  Delete Data from Manufacturer table
        private void DeleteData()
        {
            try
            {
                string[] strResult;
                string sRet = "";

                iAction = 2;
                DataTable dtUspMa = new DataTable();
                ManfInfo.MnfID = Convert.ToDecimal(iIDFromEditWindow);
                ManfInfo.MnfName = txtManufacture.Text;
                ManfInfo.MnfShortName = txtManfShortName.Text;
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                ManfInfo.DiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
                ManfInfo.SystemName = Global.gblSystemName;
                ManfInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                ManfInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                ManfInfo.LastUpdateDate = DateTime.Today;
                ManfInfo.LastUpdateTime = DateTime.Now;
                sRet = clsManf.InsertUpdateDeleteManufacturer(ManfInfo, iAction);
                if (sRet.Length > 2)
                {
                    strResult = sRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                            MessageBox.Show("Hey! There are Items Associated with this Manufacturer [" + txtManufacture.Text + "] . Please Check", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
                else
                {
                    if (Convert.ToInt32(sRet) == -1)
                        MessageBox.Show("Failed to Delete...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        ClearAll();
                }
                if (bFromEditWindow == true)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtManufacture.Text="";
            txtManfShortName.Text = "";
            btnDelete.Enabled = false;
            txtDiscountPerc.Text = "0";
            txtManufacture.Focus();
            txtManufacture.SelectionStart = txtManufacture.Text.ToString().Length;
        }
        #endregion
    }
}



