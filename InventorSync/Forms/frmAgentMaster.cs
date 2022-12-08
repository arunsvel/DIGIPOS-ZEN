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
using InventorSync.Forms;
using System.Runtime.InteropServices;

namespace InventorSync
{
    // ======================================================== >>
    // Description:Agent Master
    // Developed By:Pramod Philip
    // Completed Date & Time: 15/09/2012 5.30 PM
    // Last Edited By:Anjitha K K
    // Last Edited Date & Time:02-March-2022 12:12 PM
    // ======================================================== >>
    public partial class frmAgentMaster : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmAgentMaster(int iAgentID = 0, bool bFromEdit = false, Control Controlpassed=null, bool blnDisableMinimize = false)
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

                bFromEditWindowAgent = bFromEdit;
                iIDFromEditWindow = iAgentID;
                CtrlPassed = Controlpassed;


                this.BackColor = Global.gblFormBorderColor;
                if (iAgentID != 0)
                {
                    toggleView();
                    LoadData(iAgentID);
                    txtAgentCommission.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtAgentName.Text = CtrlPassed.Text.ToString();
                }

                txtAgentName.Focus();
                txtAgentName.SelectAll();

                if (blnDisableMinimize == true) btnMinimize.Enabled = false;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Agent" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspAgentMasterInfo AgentInfo = new UspAgentMasterInfo();
        UspAreaMasterInfo AreaInfo = new UspAreaMasterInfo();
        UspGetAgentinfo GetAgent = new UspGetAgentinfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsAgentMaster Agentinsert = new clsAgentMaster();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsAreaMaster clsArea = new clsAreaMaster();
        InventorBL.Accounts.clsLedger clsled = new InventorBL.Accounts.clsLedger();
        UspLedgerInsertInfo LedgerInfo = new UspLedgerInsertInfo();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int intLID;
        int intCommission;
        int intPOstAccounts;
        int iIDFromEditWindow;
        int iAction = 0;
        string strCheck = "";
        string strAgent = "";
        bool bFromEditWindowAgent;
        Control ctrl;
        Control CtrlPassed;
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
        private void txtAgentCode_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtAgentCode, "Please specify unique Code for Agent");
        }
        private void txtAgentName_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtAgentName, "Please specify unique name for agent");
        }
        private void txtAddress_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtAddress, "Agent address to show in print and specified area");
        }
        private void txtArea_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtArea, "Agent area to show in print and specified area");
        }
        private void txtPhone_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtPhone, "Agent phone number to show in print and specified area");
        }
        private void txtEmail_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtEmail, "Agent email to show in print and specified area");
        }
        private void txtAgentDiscount_Click(object sender, EventArgs e)
        {
            toolAgent.SetToolTip(txtAgentDiscount, "The agent Discount %,to calculate the Percentage of discount as per setting.");
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void txtAgentDiscount_KeyPress(object sender, KeyPressEventArgs e)
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
        private void txtAgentCommission_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void toggleCommission_ToggleStateChanged(object sender, ToggleStateChangedEventArgs e)
        {
            toggleView();
        }
        private void cmbAgentLederName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //intLID = cmbAgentLederName.SelectedIndex;
           //cmbAgentLederName.tag = Convert.ToString(cmbAgentLederName.SelectedValue);
        }
        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            if (txtArea.Text != "")
            {
                if (this.ActiveControl.Name != "txtArea")
                    return;
                string sQuery = "SELECT ISNULL(Area1.Area,'') + ISNULL(Area2.Area,'') As AnyWhere,Area1.Area As [Sub Area],Area2.Area As [Area],Area1.AreaID FROM tblarea Area1 FULL JOIN tblarea Area2 ON Area2.AreaID=Area1.ParentID  WHERE Area1.TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromAreaSearch, sQuery, "AnyWhere|ISNULL(Area1.Area,'')|ISNULL(Area2.Area,'')", txtArea.Location.X + 470, txtArea.Location.Y + 138, 2, 0, txtArea.Text, 3, 0, "ORDER BY Area ASC", 0, 0, "Area Search ...", 0, "200,100,0", true, "frmAreaMaster").ShowDialog();
                SendKeys.Send("{Tab}");
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
                MessageBox.Show("Enter Key Press is not working properly ? " + "\n" + ex.Message + " | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtAddress.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtPhone.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                string AreaText = txtArea.Text;
                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                if (txtArea.Text == "") txtArea.Text = "~";
                this.txtArea.TextChanged += this.txtArea_TextChanged;
                string sQuery = "SELECT ISNULL(Area1.Area,'') + ISNULL(Area2.Area,'') As AnyWhere,Area1.Area As [Sub Area],Area2.Area As [Area],Area1.AreaID FROM tblarea Area1 FULL JOIN tblarea Area2 ON Area2.AreaID=Area1.ParentID  WHERE Area1.TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromAreaSearch, sQuery, "AnyWhere|ISNULL(Area1.Area,'')|ISNULL(Area2.Area,'')", txtArea.Location.X + 470, txtArea.Location.Y + 138, 2, 0, "", 3, 0, "ORDER BY Area ASC", 0, 0, "Area Search ...", 0, "200,100,0", true, "frmAreaMaster").ShowDialog();

                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                if (txtArea.Text == "~") txtArea.Clear();
                if (txtArea.Text == "")
                    txtArea.Text = AreaText;
                this.txtArea.TextChanged += this.txtArea_TextChanged;
                SendKeys.Send("{Tab}");
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnAddArea.PerformClick();
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnEditArea.PerformClick();
            }
        }
        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtPhone.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cmbAgentLederName.Focus();
                //SendKeys.Send("{F4}");
            }
        }
        private void cmbAgentLederName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtEmail.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtAgentDiscount.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnAddLedger.PerformClick();
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnEditLedger.PerformClick();
            }
        }
        private void txtAgentDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cmbAgentLederName.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                toggleCommission.Focus();
            }
        }
        private void txtAgentCommission_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void txtAgentName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtAgentName.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                txtAgentCode.Focus();
            }
        }
        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtAgentCode.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                if (txtAddress.Text == "")
                {
                    e.SuppressKeyPress = true;
                    txtArea.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else
                {
                    if (Comm.IsCursorOnEmptyLine(txtAddress) == true)
                    {
                        e.SuppressKeyPress = true;
                        txtArea.Focus();
                        SendKeys.Send("+{DOWN}");
                    }
                }
            }
        }
        private void toggleCommission_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtAgentDiscount.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (txtAgentDiscount.Text != "")
                    {
                         if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtAgentDiscount.Text), "Agent") == false)
                         {
                            SaveData();
                         }
                         else
                         {
                             txtAgentDiscount.Text = "99";
                             txtAgentDiscount.Focus();
                             txtAgentDiscount.SelectAll();
                         }
                    }
                    else
                    {
                         txtAgentDiscount.Text = "0";
                    }
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Save..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                   Cursor.Current = Cursors.Default;
                }
            }
            Cursor.Current = Cursors.Default;
        }
        private void frmAgentMaster_Load(object sender, EventArgs e)
        {

        }
        private void frmAgentMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtAgentCode.Text != "")
                    {
                        if (txtAgentCode.Text != strCheck)
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
                //    frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper());
                //    frmEdit.Show();
                //}
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        SaveData();
                        Cursor.Current = Cursors.Default;
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        if (bFromEditWindowAgent == true)
                        {
                            if (iIDFromEditWindow > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Agent[" + txtAgentCode.Text + "] Permanently ?", Global.gblMessageCaption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    try
                                    {
                                        DeleteData();

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
                            else
                                MessageBox.Show("Default Agent [" + txtAgentCode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Cursor.Current = Cursors.Default;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Short Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Short Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void txtAgentDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                toggleCommission.Focus();
            }
        }
        private void toggleCommission_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.Focus();
            }
        }
        //For Casing
        private void txtAgentCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgentCode, true);
        }
        private void txtAgentCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgentCode);
        }
        private void txtAgentName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgentName, true);
        }
        private void txtAgentName_Leave(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (txtAgentName.Text.Length > 7)
                {
                    if (txtAgentCode.Text.Trim() == "")
                        txtAgentCode.Text = txtAgentName.Text;
                        //txtAgentCode.AppendText(txtAgentName.Text.Substring(0, 7));
                }
                else
                {
                    if (txtAgentCode.Text.Trim() == "")
                        txtAgentCode.Text = txtAgentName.Text;
                        //txtAgentCode.AppendText(txtAgentName.Text);
                }

                Comm.ControlEnterLeave(txtAgentName);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to append Agent Name to Agent Code...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtAddress_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress, true);
        }
        private void txtAddress_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress);
        }
        private void txtArea_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtArea, true);
        }
        private void txtArea_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtArea, false, false);
        }
        private void txtPhone_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPhone, true);
        }
        private void txtPhone_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPhone);
        }
        private void txtEmail_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEmail, true);
        }
        private void txtEmail_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEmail, false, false);
        }
        private void cmbAgentLederName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cmbAgentLederName, true);
        }
        private void cmbAgentLederName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cmbAgentLederName, false, false);
        }
        private void txtAgentCommission_Enter(object sender, EventArgs e)
        {
            //Comm.ControlEnterLeave(txtAgentDiscount, true);
        }
        private void txtAgentCommission_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgentCommission, false, false);
        }
        private void txtAgentDiscount_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgentDiscount, true);
        }
        private void txtAgentDiscount_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAgentDiscount.Text))
                txtAgentDiscount.Text = "0";
            else if (txtAgentDiscount.Text.TrimEnd().TrimStart() == ".")
                txtAgentDiscount.Text = ".0";
            if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtAgentDiscount.Text), "Agent") == true)
            {
                txtAgentDiscount.Text = "99";
                txtAgentDiscount.Focus();
                txtAgentDiscount.SelectAll();
            }
            Comm.ControlEnterLeave(txtAgentDiscount, false, false);
            txtAgentDiscount.Text = FormatValue(Convert.ToDouble(txtAgentDiscount.Text), true, "#.00");
        }

        private void btnAddArea_Click(object sender, EventArgs e)
        {
            try
            {
                frmAreaMaster frmArea = new frmAreaMaster(0, false, txtArea);
                frmArea.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEditArea_Click(object sender, EventArgs e)
        {
            try
            {
                frmAreaMaster frmArea = new frmAreaMaster(Convert.ToInt32(txtArea.Tag), true, txtArea);
                frmArea.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnAddLedger_Click(object sender, EventArgs e)
        {
            try
            {
                frmLedger frmLed = new frmLedger(0, false, 0,"", cmbAgentLederName);
                frmLed.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEditLedger_Click(object sender, EventArgs e)
        {
            try
            {
                frmLedger frmLed = new frmLedger(Convert.ToInt32(cmbAgentLederName.SelectedValue), true, 0, "", cmbAgentLederName);
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
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (txtAgentDiscount.Text == "")
                    txtAgentDiscount.Text = "0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtAgentDiscount.Text), "Agent") == false)
                {
                    SaveData();
                }
                else
                {
                    txtAgentDiscount.Text = "99";
                    txtAgentDiscount.Focus();
                    txtAgentDiscount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            Cursor.Current = Cursors.Default;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            if (iIDFromEditWindow > 5)
            {
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Agent[" + txtAgentCode.Text + "] Permanently ?", Global.gblMessageCaption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    try
                    {
                        DeleteData();

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
            else
                MessageBox.Show("Default Agent [" + txtAgentCode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Cursor.Current = Cursors.Default;
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
            frmEdit.ShowDialog();
            this.Visible = false;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtAgentCode.Text != "")
            {
                if (txtAgentCode.Text != strCheck)
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
            if (txtAgentName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Agent Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtAgentName.Focus();
            }
            else if(txtAgentCode.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter Agent Code", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtAgentCode.Focus();
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
        //Description :Set Toggle view Status
        private void toggleView()
        {
            if (toggleCommission.ToggleState == ToggleButtonState.Active)
            {
                intCommission = 0;
                txtAgentCommission.Enabled = false;
                txtAgentCommission.Text = intCommission.ToString();
            }
            if (toggleCommission.ToggleState == ToggleButtonState.Inactive)
            {
                intCommission = 1;
                txtAgentCommission.Enabled = true;
                txtAgentCommission.Text = intCommission.ToString();
            }
        }
        //Description :Fill Ledger Name in combobox
        private void combofill(int iSelID = 0)
        {
            Common cmbfill = new Common();
            DataTable dtLedger = new DataTable();
            dtLedger = Comm.fnGetData("SELECT -2 as  LID,'<None>' as LName FROM tblLedger UNION SELECT -1 as  LID,' <AutoCrerateLedger> ' as LName FROM tblLedger UNION SELECT DISTINCT LID as LID,LName as LName FROM tblLedger where TenantID =" + Global.gblTenantID + " ORDER BY LID").Tables[0];
            if (dtLedger.Rows.Count > 0)
            {
                Comm.LoadControl(cmbAgentLederName, dtLedger, "", false, false, "LName", "LID");
                if (iSelID != 0)
                {
                    cmbAgentLederName.SelectedValue = iSelID;
                    cmbAgentLederName.Tag = iSelID;
                    foreach (System.Data.DataRow row in dtLedger.Rows)
                    {
                        if (Convert.ToDecimal(row["LID"].ToString()) == iSelID)
                        {
                            intLID = Convert.ToInt32(row["LID"].ToString());
                        }
                    }
                }
            }
        }
        //Description :Fill Data  when Area is Select from the Grid Compact Search
        private Boolean GetFromAreaSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtArea = new DataTable();

            if (sCompSearchData.Length > 0)
            {
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        AreaInfo.AreaID = Convert.ToInt32(sCompSearchData[0].ToString());
                        AreaInfo.TenantID = Global.gblTenantID;
                        dtArea = clsArea.GetAreaMaster(AreaInfo);
                        if (dtArea.Rows.Count > 0)
                        {
                            this.txtArea.TextChanged -= this.txtArea_TextChanged;
                            txtArea.Text = dtArea.Rows[0]["Area"].ToString();
                            this.txtArea.TextChanged += this.txtArea_TextChanged;
                            txtArea.Tag = dtArea.Rows[0]["AreaID"].ToString();
                        }
                        return true;
                    }
                    else
                    {
                        this.txtArea.TextChanged -= this.txtArea_TextChanged;
                        txtArea.Text = sCompSearchData[1].ToString();
                        this.txtArea.TextChanged += this.txtArea_TextChanged;
                        AreaInfo.AreaID = Convert.ToInt32(sCompSearchData[0].ToString());
                        return true;
                    }
                }
            }
            else
                return false;
        }
        //Description :Set Area DefaultValues
        private void SetDefaultValue()
        {
            if (string.IsNullOrEmpty(txtArea.Text))
            {
                txtArea.Tag = 1;
                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                txtArea.Text = Comm.fnGetData("Select Area From tblArea Where AreaID = '" + txtArea.Tag + "'").Tables[0].Rows[0][0].ToString();
                this.txtArea.TextChanged += this.txtArea_TextChanged;
            }
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetAgent.AgentID = iSelectedID;
            GetAgent.TenantID = Global.gblTenantID;
            dtLoad = clsAgent.GetAgentMaster(GetAgent);
            if (dtLoad.Rows.Count > 0)
            {
                txtAgentCode.Text = dtLoad.Rows[0]["AgentCode"].ToString();
                strCheck = dtLoad.Rows[0]["AgentCode"].ToString();
                txtAgentName.Text = dtLoad.Rows[0]["AgentName"].ToString();
                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                txtArea.Text = dtLoad.Rows[0]["Area"].ToString();
                txtArea.Tag = dtLoad.Rows[0]["AreaID"].ToString();
                this.txtArea.TextChanged += this.txtArea_TextChanged;
                txtArea.Tag = dtLoad.Rows[0]["AreaID"].ToString();
                txtAddress.Text = dtLoad.Rows[0]["ADDRESS"].ToString();
                txtPhone.Text = dtLoad.Rows[0]["PHONE"].ToString();
                txtEmail.Text = dtLoad.Rows[0]["EMAIL"].ToString();
                if (Convert.ToDouble(dtLoad.Rows[0]["Commission"]) == 0)
                {
                    toggleCommission.ToggleState = ToggleButtonState.Inactive;
                }
                else
                {
                    toggleCommission.ToggleState = ToggleButtonState.Active;
                }
                txtAgentCommission.Text = dtLoad.Rows[0]["Commission"].ToString();
                decimal DiscPer = Convert.ToDecimal(dtLoad.Rows[0]["AgentDiscount"].ToString());
                txtAgentDiscount.Text = FormatValue(Convert.ToDouble(DiscPer), true, "#.00");
                combofill();
                cmbAgentLederName.ValueMember = "LID";
                cmbAgentLederName.DisplayMember = "LName";
                cmbAgentLederName.Text = dtLoad.Rows[0]["LedgerName"].ToString();
                cmbAgentLederName.Tag= dtLoad.Rows[0]["LID"].ToString();
                cmbAgentLederName.SelectedValue = dtLoad.Rows[0]["LID"].ToString();
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
                    AgentInfo.AgentID = Comm.gfnGetNextSerialNo("tblAgent", "AgentID");
                    if (AgentInfo.AgentID < 6)
                        AgentInfo.AgentID = 6;
                }
                else
                    AgentInfo.AgentID = iIDFromEditWindow;
                AgentInfo.AgentCode = txtAgentCode.Text;
                AgentInfo.AgentName = txtAgentName.Text;
                AgentInfo.Area = txtArea.Text;
                AgentInfo.AreaID = Convert.ToDecimal(txtArea.Tag);
                intLID =cmbAgentLederName.SelectedIndex;
                if (intLID != 1)
                {
                    intPOstAccounts = 1;
                }
                else
                {
                    intPOstAccounts = 0;
                }
                AgentInfo.blnPOstAccounts = intPOstAccounts;
                AgentInfo.ADDRESS = txtAddress.Text;
                AgentInfo.LOCATION = txtArea.Text;
                AgentInfo.PHONE = txtPhone.Text;
                AgentInfo.WEBSITE = "";
                AgentInfo.EMAIL = txtEmail.Text;
                AgentInfo.BLNROOMRENT = 0;
                AgentInfo.BLNSERVICES = 0;
                if (intCommission != 0)
                {
                    AgentInfo.blnItemwiseCommission = 1;
                    AgentInfo.Commission = Convert.ToDecimal(txtAgentCommission.Text);
                }
                else
                {
                    intCommission = 0;
                    AgentInfo.Commission = 0;
                }
                if (txtAgentDiscount.Text == "")
                    AgentInfo.AgentDiscount = 0;
                else
                    AgentInfo.AgentDiscount = Convert.ToDecimal(txtAgentDiscount.Text);
                if (string.IsNullOrEmpty(cmbAgentLederName.Text))
                    cmbAgentLederName.Tag = "-1";
                AgentInfo.LID =Convert.ToDecimal(cmbAgentLederName.SelectedValue);
                AgentInfo.SystemName = Environment.MachineName;
                AgentInfo.UserID = Global.gblUserID;
                AgentInfo.LastUpdateDate = DateTime.Today;
                AgentInfo.LastUpdateTime = DateTime.Now;
                AgentInfo.TenantID = Global.gblTenantID;
                if(AgentInfo.LID == -1)
                {
                    if (iAction == 0)
                        LedgerInfo.LID = Comm.gfnGetNextSerialNo("tblLedger", "LID");
                    else
                    {
                        string strLID= Comm.fnGetData("Select ISNULL(LID,0) as LID From tblagent Where AgentID = '" + AgentInfo.AgentID + "'").Tables[0].Rows[0][0].ToString();
                        LedgerInfo.LID = Convert.ToDecimal(strLID);
                    }
                    LedgerInfo.LName = txtAgentName.Text;
                    LedgerInfo.LAliasName = txtAgentCode.Text;
                    LedgerInfo.MobileNo = txtPhone.Text;
                    LedgerInfo.Email = txtEmail.Text;
                    LedgerInfo.GroupName = "LEDGER";
                    LedgerInfo.EntryDate = DateTime.Today;
                    LedgerInfo.DOB = Convert.ToDateTime(null);
                    LedgerInfo.Area = txtArea.Text;
                    LedgerInfo.AreaID = Convert.ToDecimal(txtArea.Tag);
                    AgentInfo.LID = LedgerInfo.LID;
                    LedgerInfo.GSTType = "";
                    LedgerInfo.Address = txtAddress.Text;
                    string strStateid= Comm.fnGetData("Select StateId From tblStates Where StateCode = '" + AppSettings.StateCode + "'").Tables[0].Rows[0][0].ToString();
                    LedgerInfo.StateID =Convert.ToDecimal(strStateid);
                    LedgerInfo.TaxNo = "";
                    LedgerInfo.DiscPer = 0;
                    LedgerInfo.PLID = 0;
                    LedgerInfo.OpBalance = 0;
                    LedgerInfo.Type = "";
                    LedgerInfo.EntryDate = DateTime.Today;
                    LedgerInfo.DOB = DateTime.Today;
                    LedgerInfo.AgentID = 1;
                    LedgerInfo.AccountGroupID = 0;
                    LedgerInfo.ActiveStatus = 0;
                    string name = Environment.MachineName;
                    LedgerInfo.SystemName = name;
                    LedgerInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                    LedgerInfo.LastUpdateDate = DateTime.Today;
                    LedgerInfo.LastUpdateTime = DateTime.Now;
                    LedgerInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);

                    clsled.InsertUpdateDeleteLedger(LedgerInfo, iAction);

                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                MessageBox.Show("Duplicate Entry, User has restricted to enter duplicate values in the Ledger name(" + txtAgentName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        }
                    }
                }
                 strRet = Agentinsert.InsertUpdateDeleteAgentMaster(AgentInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            if (strResult[1].ToString().Contains("IX_tblAgent"))
                            {
                                MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Agent Code (" + txtAgentCode.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                txtAgentCode.Focus();
                                txtAgentCode.SelectAll();
                            }
                            else
                            {
                                MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Agent Name (" + txtAgentName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtAgentName.Focus();
                                txtAgentName.SelectAll();
                            }
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                        Comm.MessageboxToasted("Agent", "Agent saved successfully");
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        if (CtrlPassed is TextBox)
                        {
                            CtrlPassed.Text = txtAgentName.Text;
                            CtrlPassed.Tag = AgentInfo.AgentID;
                            CtrlPassed.Name = txtAgentName.Name;
                            CtrlPassed.Focus();
                        }
                        this.Close();
                    }
                    else
                    {
                        ClearAll();
                        toggleView();
                    }
                    Comm.MessageboxToasted("Agent", "Agent saved successfully");
                    if (bFromEditWindowAgent == true)
                    {
                        this.Close();
                    }
                    
                }
            }
        }
        //Description :  Delete Data from Unit table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            AgentInfo.AgentID = iIDFromEditWindow;
            AgentInfo.AgentCode = txtAgentCode.Text;
            AgentInfo.AgentName = txtAgentName.Text;
            AgentInfo.Area = txtArea.Text;
            AgentInfo.AreaID = Convert.ToDecimal(txtArea.Tag);
            AgentInfo.Commission = 0;
            AgentInfo.blnPOstAccounts = 0;
            AgentInfo.ADDRESS = txtAddress.Text;
            AgentInfo.LOCATION = txtArea.Text;
            AgentInfo.PHONE = txtPhone.Text;
            AgentInfo.WEBSITE = "";
            AgentInfo.EMAIL = txtEmail.Text;
            AgentInfo.BLNROOMRENT = 0;
            AgentInfo.BLNSERVICES = 0;
            AgentInfo.blnItemwiseCommission = 0;
            AgentInfo.AgentDiscount = Convert.ToDecimal(txtAgentDiscount.Text);
            AgentInfo.LID = Convert.ToDecimal(cmbAgentLederName.SelectedValue);
            AgentInfo.SystemName = Environment.MachineName;
            AgentInfo.UserID = Global.gblUserID;
            AgentInfo.LastUpdateDate = DateTime.Today;
            AgentInfo.LastUpdateTime = DateTime.Now;
            AgentInfo.TenantID = Global.gblTenantID;
            strRet = clsAgent.InsertUpdateDeleteAgentMaster(AgentInfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are entries Associated with this Agent [" + txtAgentName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (bFromEditWindowAgent == true)
            {
                this.Close();
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtAgentCode.Text = "";
            txtAgentName.Text = "";
            txtAddress.Text = "";
            txtArea.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAgentDiscount.Text = "";
            txtAgentDiscount.Text = "0";
            txtAgentCommission.Text = "";
            cmbAgentLederName.SelectedIndex = 0;
            btnDelete.Enabled = false;
            txtArea.Tag = -2;
            combofill();
            cmbAgentLederName.SelectedValue = -2;
            cmbAgentLederName.Tag = -2;
            txtAgentName.Focus();
            SetDefaultValue(); 
        }
        #endregion

        private void frmAgentMaster_Activated(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();
                    combofill();
                    toggleView();
                    cmbAgentLederName.SelectedIndex = 0;
                    SetDefaultValue();
                    //this.Visible = false;
                    //this.Show();
                    //Application.DoEvents();
                    txtAgentName.Focus();
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtAgentName.Text = CtrlPassed.Text.ToString();
                }

                txtAgentName.Select();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load Agent  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}   
