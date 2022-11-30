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
using Syncfusion.Windows.Forms.Tools;
using System.Runtime.InteropServices;

namespace InventorSync
{
    // ======================================================== >>
    // Description:Tax Mode Creation
    // Developed By:Anjitha K K
    // Completed Date & Time: 03-Jan-2022 04:00 PM
    // Last Edited By:
    // Last Edited Date & Time:
    // ======================================================== >>

    public partial class frmTaxMode : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmTaxMode(int iTaxID = 0, bool bFromEdit = false)
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
                lblFind.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;
                lblFind.ForeColor = Color.Black;

                btnSave.Image = global::InventorSync.Properties.Resources.save240402;
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

            iIDFromEditWindow = iTaxID;
            this.BackColor = Global.gblFormBorderColor;
            if (iTaxID != 0)
            {
                LoadData(iTaxID); 
            }
            else
            {
                //btnDelete.Enabled = false;
            }
            List();
            bFromEditWindowTax = bFromEdit;
            Cursor.Current = Cursors.Default;

            MessageBox.Show("Taxmode editing is limited to some properties. " + "\n" + "New taxmode creation is suspended. ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        #region "VARIABLES -------------------------------------------- >>"
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetTaxModeInfo GetTaxMode = new UspGetTaxModeInfo();
        UspTaxModeInsertInfo TaxModeInfo = new UspTaxModeInsertInfo();
        clsTaxMode clsTaxMode = new clsTaxMode();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iAction = 0;
        int iIDFromEditWindow;
        int iActive = 0;
        string strCheck;
        string taxID;
        Control ctrl;
        bool bFromEditWindowTax;
        Boolean bList=false;
        int iListView1LostFocusItem;
        int taxModeID;
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
        private void txtTaxMode_Click(object sender, EventArgs e)
        {
            toolTaxmode.SetToolTip(txtTaxMode, "Tax Mode to show in print and specified area");
        }
        private void cmbCalculationMode_Click(object sender, EventArgs e)
        {
            toolTaxmode.SetToolTip(cmbCalculationMode, "Calculation Mode to set specified Area");
        }
        private void txtSortOrder_Click(object sender, EventArgs e)
        {
            toolTaxmode.SetToolTip(txtSortOrder, "Sort Order to show in print and specified area");
        }

        private void frmTaxMode_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (iIDFromEditWindow == 0)
            {
                ClearAll();
                this.Show();
                Application.DoEvents();
                FillTransSortOrder();
                togglebtnActive.ToggleState = ToggleButtonState.Active;
            }
            txtTaxMode.Select();
            txtTaxMode.SelectAll();
            Cursor.Current = Cursors.Default;
        }
        private void frmTaxMode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtTaxMode.Text != "")
                    {
                        if (txtTaxMode.Text != strCheck)
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
                else if (e.KeyCode == Keys.F3)//Create
                {
                    ClearAll();
                    //btnDelete.Enabled = false;
                    bList = false;
                    this.Show();
                    Application.DoEvents();
                    togglebtnActive.ToggleState = ToggleButtonState.Active;
                    txtTaxMode.Select();
                    txtTaxMode.SelectAll();
                }
                else if (e.KeyCode == Keys.F4)//Edit
                {
                    this.Close();
                    frmTaxMode frmTax = new frmTaxMode(0, true);
                    frmTax.Show();
                }
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                        SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F6)//Print
                {

                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    try
                    {
                        if (bFromEditWindowTax == true)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Tax Mode [" + txtTaxMode.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Tax Mode [" + txtTaxMode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Cursor.Current = Cursors.Default;
                        }
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
        private void txtTaxMode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                   Cursor.Current = Cursors.WaitCursor;
                    if (e.Shift == true && e.KeyCode == Keys.Enter)
                    {
                    txtTaxMode.Focus();
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                    cmbCalculationMode.Focus();
                    //SendKeys.Send("{F4}");
                    }
                   Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void cmbCalculationMode_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter))
            {
                txtTaxMode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtSortOrder.Focus();
            }
        }
        private void txtSortOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                cmbCalculationMode.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                togglebtnActive.Focus();
            }
        }
        private void togglebtnActive_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtSortOrder.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                SaveData();
            }
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
                MessageBox.Show("Sort Order numeric key entry Failed .." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtSortOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)//For Tab Key
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                togglebtnActive.Focus();
            }
        }
        private void togglebtnActive_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.Focus();
            }
        }

        //List Grid for show Recently Modified Data
        private void lvedit_Click(object sender, EventArgs e)//List Grid Click
        {
            if (bList == false && bFromEditWindowTax == false)
            {
                if (lvedit.SelectedItems.Count > 0)
                {
                    DialogResult dlgResult = MessageBox.Show("Do you want to enable edit mode", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult.Equals(DialogResult.Yes))
                    {
                        EditmodeList();
                    }
                }
            }
            else
            {
                EditmodeList();
            }
        }
        private void lvedit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (bList == false && bFromEditWindowTax == false)
                {
                    if (lvedit.SelectedItems.Count > 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("Do you want to enable edit mode", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult.Equals(DialogResult.Yes))
                        {
                            EditmodeList();
                        }
                    }
                }
                else
                {
                    EditmodeList();
                }
            }
        }
        private void lvedit_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // If this item is the selected item
            if (e.Item.Selected)
            {
                // If the selected item just lost the focus
                if (iListView1LostFocusItem == e.Item.Index)
                {
                    // selected item when it has focus)
                    e.Item.ForeColor = Color.Black;
                    e.Item.BackColor = Color.LightBlue;

                    // Indicate that this action does not need to be performed
                    iListView1LostFocusItem = -1;
                }
                else if (lvedit.Focused)  // If the selected item has focus
                {
                    // Set the colors to the normal colors for a selected item
                    e.Item.ForeColor = SystemColors.HighlightText;
                    e.Item.BackColor = SystemColors.Highlight;
                }
            }
            else
            {
                // Set the normal colors for items that are not selected
                e.Item.ForeColor = lvedit.ForeColor;
                e.Item.BackColor = lvedit.BackColor;
            }
            e.DrawBackground();
            e.DrawText();
        }
        private void lvedit_Leave(object sender, EventArgs e)
        {
            iListView1LostFocusItem = lvedit.FocusedItem.Index;
        }

        private void txtTaxMode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTaxMode, true);
        }
        private void txtTaxMode_Leave(object sender, EventArgs e)//Casing
        {
            Comm.ControlEnterLeave(txtTaxMode);
        }
        private void txtSortOrder_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, true);
        }
        private void txtSortOrder_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder);
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearAll();
            //btnDelete.Enabled = false;
            bList = false;
            this.Show();
            Application.DoEvents();
            togglebtnActive.ToggleState = ToggleButtonState.Active;
            txtTaxMode.Select();
            txtTaxMode.SelectAll();
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
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Tax Mode[" + txtTaxMode.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Tax Mode [" + txtTaxMode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
                frmEdit.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtTaxMode.Text != "")
            {
                if (txtTaxMode.Text != strCheck)
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

            if (txtTaxMode.Text.Trim() == "")
            {
                bValidate = false;

                MessageBox.Show("Please enter the Tax Mode", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                txtTaxMode.Focus();
            }
            else
            {
                if (txtSortOrder.Text == "")
                    txtSortOrder.Text = "0";

                txtTaxMode.Text = txtTaxMode.Text.Replace("'", "\"");
            }

            return bValidate;
        }
        //Description : Set Sort Order in textbox
        private void FillTransSortOrder()
        {
            DataTable dtTransOrder = new DataTable();
            dtTransOrder = Comm.fnGetData("SELECT MAX(ISNULL(SortNo, 0)) + 1 as SortOrder FROM tblTaxMode WHERE TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtTransOrder.Rows.Count > 0)
            {
                txtSortOrder.Text = dtTransOrder.Rows[0]["SortOrder"].ToString();
            }
        }
        //Description : Show Last Modified Data in List
        private void List()
        {
            lvedit.Items.Clear();
            GetTaxMode.TenantID = Convert.ToDecimal(Global.gblTenantID);
            DataTable dtlist = new DataTable();
            dtlist = clsTaxMode.GetTaxModeLastUpdateDetails(GetTaxMode);
            List<clsTaxMode> listed = new List<clsTaxMode>();
            for (int i = 0; i < dtlist.Rows.Count; i++)
            {
                DataRow drow = dtlist.Rows[i];
                ListViewItem lvi = new ListViewItem(drow["TaxMode"].ToString());
                if (!string.IsNullOrEmpty(drow["LastUpdateTime"].ToString()))
                    lvi.SubItems.Add(Convert.ToDateTime(drow["LastUpdateTime"].ToString()).ToString("dd-MMM-yyyy hh:mm tt"));
                lvi.SubItems.Add(drow["UserName"].ToString());
                lvi.SubItems.Add(drow["TaxModeID"].ToString());
                lvedit.Items.Add(lvi);
            }
            lvedit.Items[0].Selected = true;
        }
        //Description : Edit Last Modified Data in List
        private void EditmodeList()
        {
            try
            {
                ListViewItem item = lvedit.SelectedItems[0];
                taxID = item.SubItems[2].Text;
                taxModeID = Convert.ToInt32(taxID);
                iIDFromEditWindow = taxModeID;
                //btnDelete.Enabled = true;
                bList = true;
                LoadData(taxModeID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not edit taxmode " + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }
        //Description : Refresh List
        private void RefreshList()
        {
            List();
            lvedit.Refresh();
            if (bList == true)
            {
                iAction = 0;
            }
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                int CalculationMode;
                Cursor.Current = Cursors.WaitCursor;
                DataTable dtLoad = new DataTable();
                GetTaxMode.TaxModeID = Convert.ToDecimal(iSelectedID);
                GetTaxMode.TenantID = Convert.ToDecimal(Global.gblTenantID);
                dtLoad = clsTaxMode.GetTaxMode(GetTaxMode);
                if (dtLoad.Rows.Count > 0)
                {
                    txtTaxMode.Text = dtLoad.Rows[0]["TaxMode"].ToString();
                    strCheck = dtLoad.Rows[0]["TaxMode"].ToString();
                    CalculationMode=Convert.ToInt32(dtLoad.Rows[0]["CalculationID"]);
                    cmbCalculationMode.SelectedIndex = CalculationMode;
                    txtSortOrder.Text = dtLoad.Rows[0]["SortNo"].ToString();
                    if (Convert.ToInt32(dtLoad.Rows[0]["ActiveStatus"].ToString()) == 1)
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    iAction = 1;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Edit view is not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    TaxModeInfo.TaxModeID = Comm.gfnGetNextSerialNo("tblTaxMode", "TaxModeID");
                    if(TaxModeInfo.TaxModeID<6)
                    TaxModeInfo.TaxModeID = 6;
                }
                else
                    TaxModeInfo.TaxModeID = Convert.ToDecimal(iIDFromEditWindow);
                TaxModeInfo.TaxMode = txtTaxMode.Text;
                DataTable dtTaxmodeInsert = new DataTable();
                TaxModeInfo.CalculationID = cmbCalculationMode.SelectedIndex;
                TaxModeInfo.SortNo = Convert.ToDecimal(txtSortOrder.Text);
                if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActive = 1;
                TaxModeInfo.ActiveStatus = iActive;
                TaxModeInfo.SystemName = Global.gblSystemName;
                TaxModeInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                TaxModeInfo.LastUpdateDate = DateTime.Today;
                TaxModeInfo.LastUpdateTime = DateTime.Now;
                TaxModeInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                strRet = clsTaxMode.InsertUpdateDeleteTaxMode(TaxModeInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            MessageBox.Show("Duplicate Entry,User has restricted to enter duplicate values in the Tax Mode(" + txtTaxMode.Text + ")", Global.gblMessageCaption,MessageBoxButtons.OK,MessageBoxIcon.Error);
                            txtTaxMode.Focus();
                            txtTaxMode.SelectAll();
                        }
                        else
                             MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                         if (bFromEditWindowTax == true)
                         {
                            this.Close();
                         }
                         RefreshList();
                         ClearAll();
                         Comm.MessageboxToasted("Tax Mode", "Tax Mode saved successfully");
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save! ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                       if (bFromEditWindowTax == true)
                       {
                          this.Close();
                       }
                          RefreshList();
                          ClearAll();
                          Comm.MessageboxToasted("Tax Mode", "Tax Mode saved successfully");
                    }
                }
            }
        }
        //Description :  Delete Data from Tax Mode table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtTaxMode = new DataTable();
            TaxModeInfo.TaxModeID = Convert.ToDecimal(iIDFromEditWindow);
            TaxModeInfo.TaxMode = txtTaxMode.Text;
            TaxModeInfo.CalculationID = cmbCalculationMode.SelectedIndex;
            TaxModeInfo.SortNo = Convert.ToDecimal(txtSortOrder.Text);
            TaxModeInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            TaxModeInfo.SystemName = Environment.MachineName;
            TaxModeInfo.UserID = Global.gblUserID;
            TaxModeInfo.LastUpdateDate = DateTime.Today;
            TaxModeInfo.LastUpdateTime = DateTime.Now;
            strRet = clsTaxMode.InsertUpdateDeleteTaxMode(TaxModeInfo, iAction);
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
                   MessageBox.Show("Failed to Delete...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                   ClearAll();
            }
            ClearAll();
            if (bFromEditWindowTax == true)
            {
               this.Close();
            }
            if(bList == true)
            {
               List();
               lvedit.Refresh();
               iAction = 0;
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtTaxMode.Clear();
            cmbCalculationMode.SelectedIndex = 0;
            txtSortOrder.Clear();
            togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            FillTransSortOrder();
            txtTaxMode.Focus();
        }
        #endregion
    }
}
