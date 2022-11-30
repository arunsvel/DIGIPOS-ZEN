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
using InventorSync.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;

namespace InventorSync
{
    public partial class frmLedger : Form, IMessageFilter
    {

        // ======================================================== >>
        // Description:  Ledger Creation          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 28/12/2021   & 12:03 PM
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
        public frmLedger(int iLedger = 0, bool bFromEdit = false, decimal AccGrpID = 0,string AccGroup="", Control Controlpassed = null, bool blnDisableMinimize = false)
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

                iIDFromEditWindow = iLedger;
                bFromEditWindowLedger = bFromEdit;
                strSUPORCUS = AccGroup;
                dAccGroupDefaultEmployee = AccGrpID;
                CtrlPassed = Controlpassed;

                if (strSUPORCUS == "SUPPLIER")
                    cboGsttyp.SelectedIndex = 0;
                else if (strSUPORCUS == "CUSTOMER")
                    cboGsttyp.SelectedIndex = 1;
                else
                    cboGsttyp.SelectedIndex = 0;

                this.BackColor = Global.gblFormBorderColor;
                if (iLedger != 0)
                {
                    FillTreeview();
                    Settreeview();
                    LoadData(iLedger);
                    btnDelete.Visible = true;
                    lblDelete.Visible = true;
                    txtledgerName.Focus();
                    //txtledgerName.SelectAll();
                    txtledgerName.SelectionStart = txtledgerName.Text.ToString().Length;
                }
                else
                {
                    FillStates();
                    LoadPriceList();
                    btnDelete.Enabled = false;
                    cboGroup.SelectedIndex = 2;
                    cboPriceList.SelectedIndex = 0;
                    lblAdd.Visible = false;
                    txtledgerAdd.Visible = false;
                    trvwParentGroup.Size = new Size(260, 243);
                    string sStateCode = AppSettings.StateCode;
                    togglebtnActive.ToggleState = ToggleButtonState.Active;
                    //string[] strArrState = sStateCode.Split('-');
                    //if (strArrState.Length > 0)
                    //{
                    //    sStateCode = strArrState[1];
                    //}
                    //int iStateID = Convert.ToInt32(Comm.fnGetData("Select StateID From tblStates Where StateCode = '" + sStateCode + "'").Tables[0].Rows[0][0].ToString());
                    cboState.SelectedValue = sStateCode;
                }
                ApplicationSettings();
                if (dAccGroupDefaultEmployee == 20)
                {
                    dGroupID = 20;
                    SetTreeviewDefaultValue();
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtledgerName.Text = CtrlPassed.Text.ToString();
                    txtledgerAliasName.Text = CtrlPassed.Text.ToString();
                }

                txtledgerName.Focus();
                txtledgerName.Select();
                txtledgerName.SelectionStart = txtledgerName.Text.ToString().Length;

                if (blnDisableMinimize == true) btnMinimize.Enabled = false;

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
        InventorBL.Accounts.clsLedger clsled = new InventorBL.Accounts.clsLedger();
        UspLedgerInsertInfo LedgerInfo = new UspLedgerInsertInfo();
        UspGetLedgerInfo GetLedgerInfo = new UspGetLedgerInfo();
        UspAreaMasterInfo AreaInfo = new UspAreaMasterInfo();
        //Cls
        clsAreaMaster clsArea = new clsAreaMaster();
        UspGetAgentinfo AgentInfo = new UspGetAgentinfo();
        clsAgentMaster clsAgent = new clsAgentMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        int itvwParentID;
        Control ctrl;
        string strCheck;
        String strGroupName, strSUPORCUS;
        string strAccGroup = ""; 
        decimal dStateID=0;
        decimal dAccGroupID = 0, dAccGroupDefaultEmployee = 0;
        bool bFromEditWindowLedger;
        String SR1, SR2, SR3, SR4, SR5, MRP;
        Control CtrlPassed;
        decimal dGroupID = 0;
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
        private void txtledgerName_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerName, "Please specify unique name for ledger");
        }
        private void txtledgerAliasName_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerAliasName, "Ledger alias name to show print and specified area");
        }
        private void txtledgerAdd_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerAdd, "Specify ledger address to show print and specified area");
        }
        private void txtledgerMob_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerMob, "Specify ledger mobile no to show print and specified area");
        }
        private void txtledgerEmail_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerEmail, "Specify ledger email to show print and specified area");
        }
        private void txtledgerTaxreg_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerTaxreg, "Specify ledger tax registration number to show print and specified area");
        }
        private void txtledgerOpbal_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtledgerOpbal, "Specify opening balance for ledger");
        }
        private void cboLedgerOpTyp_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(cboLedgerOpTyp, "Select one  opening type for ledger");
        }
        private void cboGroup_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(cboGroup, "Select one  account group for ledger");
        }
        private void cboGsttyp_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(cboGsttyp, "Select one  gst type for ledger");
        }
        private void cboState_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(cboState, "Select one state for ledger");
        }
        private void txtArea_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtArea, "Select one Area for ledger");
        }
        private void txtAgent_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(txtAgent, "Select one Agent for ledger");
        }
        private void trvwParentGroup_Click(object sender, EventArgs e)
        {
            toolLedger.SetToolTip(trvwParentGroup, "Select one Group for ledger");
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
        private void txtledgerAliasName_KeyDown(object sender, KeyEventArgs e)
        {

            ctrl = (Control)sender;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtledgerName.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                //if (strSUPORCUS == "SUPPLIER" || strSUPORCUS == "CUSTOMER")
                //{
                //    txtledgerAdd.Focus();
                //}
                //else
                //{

                    if (cboGroup.Enabled == true)
                    cboGroup.Focus();
                   else
                    cboGsttyp.Focus();
                    SendKeys.Send("{F4}");
               // }
                
            }
            Cursor.Current = Cursors.Default;
        }
        private void cboGroup_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtledgerAliasName.Focus();
            }

            else if (e.KeyCode == Keys.Enter)
            {
                cboGsttyp.Focus();
                SendKeys.Send("{F4}");
            }
            Cursor.Current = Cursors.Default;
        }
        private void cboGsttyp_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (cboGroup.Enabled == true)
                    cboGroup.Focus();
                else
                    txtledgerAliasName.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                trvwParentGroup.Focus();
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtledgerEmail_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtledgerMob.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                cboState.Focus();
                SendKeys.Send("{F4}");
            }
            Cursor.Current = Cursors.Default;
        }
        private void cboState_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtledgerEmail.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtledgerTaxreg.Focus();
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtledgerTaxreg_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboState.Focus();

            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                if (txtArea.Visible == true)
                {
                    txtArea.Focus();
                    SendKeys.Send("+{DOWN}");
                }

                else if (txtAgent.Visible == true)
                {
                    txtAgent.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else
                    txtledgerOpbal.Focus();

            }
            Cursor.Current = Cursors.Default;
        }
        private void txtledgerDiffopbal_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    cboLedgerOpTyp.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void cboLedgerOpTyp_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtledgerOpbal.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtledgerDiffopbal.Focus();
            }
            Cursor.Current = Cursors.Default;
        }
        private void trvwParentGroup_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboGsttyp.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtledgerAdd.Focus();
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtledgerAdd_KeyDown(object sender, KeyEventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                //if (strSUPORCUS == "SUPPLIER" || strSUPORCUS == "CUSTOMER")
                //{
                //    txtledgerAliasName.Focus();
                //}
                //else
                    trvwParentGroup.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                if (txtledgerAdd.Text == "")
                {
                    SendKeys.Send("{TAB}");
                    e.SuppressKeyPress = true;
                }
                else
                {
                    if (Comm.IsCursorOnEmptyLine(txtledgerAdd) == true)
                    {
                        SendKeys.Send("{TAB}");
                        e.SuppressKeyPress = true;
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboPriceList.Focus();

            }
            else if (e.KeyCode == Keys.Down)
            {
                string strAreaText = txtArea.Text;

                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                if (txtArea.Text == "") txtArea.Text = "~";
                this.txtArea.TextChanged += this.txtArea_TextChanged;
                string sQuery = "SELECT ISNULL(Area1.Area,'') + ISNULL(Area2.Area,'') As AnyWhere,Area1.Area As [Sub Area],Area2.Area As [Area],Area1.AreaID FROM tblarea Area1 FULL JOIN tblarea Area2 ON Area2.AreaID=Area1.ParentID  WHERE Area1.TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromAreaSearch, sQuery, "AnyWhere|ISNULL(Area1.Area,'')|ISNULL(Area2.Area,'')", txtArea.Location.X + 477, txtArea.Location.Y + 100, 2, 0, "", 3, 0, "ORDER BY Area ASC", 0, 0, "Area Search ...", 0, "200,100,0", true, "frmAreaMaster").ShowDialog();
                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                if (txtArea.Text == "~") txtArea.Clear();
                if (txtArea.Text == "")
                    txtArea.Text = strAreaText;
                this.txtArea.TextChanged += this.txtArea_TextChanged;
                //SendKeys.Send("{Tab}");
                //SendKeys.Send("+{DOWN}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtAgent.Visible == true)
                {
                    txtAgent.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else
                    txtledgerOpbal.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                this.ActiveControl.Name = btnAddArea.Name;
                btnAddArea.PerformClick();
            }
            else if (e.KeyCode == Keys.F4)
            {
                this.ActiveControl.Name = btnEditLArea.Name;
                btnEditLArea.PerformClick();
            }
        }
        private void txtAgent_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtArea.Visible == true)
                    txtArea.Focus();
                else
                    cboPriceList.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                string strAgentText = txtAgent.Text;
                this.txtAgent.TextChanged -= this.txtAgent_TextChanged;
                if (txtAgent.Text == "") txtAgent.Text = "~";
                this.txtAgent.TextChanged += this.txtAgent_TextChanged;
                string sQuery = "SELECT ISNULL(AgentName,'') + ISNULL(AgentCode,'') +  ISNULL(ADDRESS,'') + ISNULL(Area,'') As AnyWhere,AgentName As [Agent Name],AgentCode As [Agent Code],ADDRESS As [ADDRESS],Area,AgentID FROM tblAgent WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromAgentSearch, sQuery, "AnyWhere|ISNULL(AgentName,'')|ISNULL(AgentCode,'')|ISNULL(ADDRESS,'')|ISNULL(Area,'')", txtAgent.Location.X + 477, txtAgent.Location.Y + 100, 4, 0, "", 5, 0, "ORDER BY AgentName ASC", 0, 0, "Agent Search ...", 0, "100,150,150,100,0", true, "frmAgent").ShowDialog();
                this.txtAgent.TextChanged -= this.txtAgent_TextChanged;
                if (txtAgent.Text == "~") txtAgent.Clear();
                if (txtAgent.Text == "")
                    txtAgent.Text = strAgentText;
                this.txtAgent.TextChanged += this.txtAgent_TextChanged;
               // SendKeys.Send("{Tab}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtledgerOpbal.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                this.ActiveControl.Name = btnAddAgent.Name;
                btnAddAgent.PerformClick();
            }
            else if (e.KeyCode == Keys.F4)
            {
                this.ActiveControl.Name = btnEditAgent.Name;
                btnEditAgent.PerformClick();
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtledgerOpbal_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtAgent.Visible == true)
                    txtAgent.Focus();
                else if (txtArea.Visible == true)
                    txtArea.Focus();
                else
                    cboPriceList.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                cboLedgerOpTyp.Focus();
            }
            Cursor.Current = Cursors.Default;
        }
        private void cboPriceList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtdiscPer.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtArea.Visible == true)
                    txtArea.Focus();
                else if (txtAgent.Visible == true)
                    txtAgent.Focus();
                else
                    txtledgerOpbal.Focus();
                SendKeys.Send("{DOWN}");
            }
            else
                return;
        }
        private void frmLedger_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
               
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();

                    if (CtrlPassed != null && iIDFromEditWindow == 0)
                    {
                        txtledgerName.Text = CtrlPassed.Text.ToString();
                        txtledgerAliasName.Text = CtrlPassed.Text.ToString();
                    }
                    txtledgerName.SelectionStart = txtledgerName.Text.ToString().Length;

                    this.Show();
                    Application.DoEvents();
                    txtledgerName.Focus();
                }
                FillTreeview();
                SetTreeviewDefaultValue();
                if (cboGroup.SelectedIndex == 2)
                {
                    txtledgerAdd.Visible = false;
                    lblAdd.Visible = false;
                    trvwParentGroup.Size = new Size(260, 243);
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtledgerName.Text = CtrlPassed.Text.ToString();
                    txtledgerAliasName.Text = CtrlPassed.Text.ToString();
                }

                trvwParentGroup.SelectedNode = Comm.GetNodeByText(trvwParentGroup.Nodes, strAccGroup);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load Ledger  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void frmLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtledgerName.Text != "")
                    {
                        if (txtledgerName.Text != strCheck)
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
                            SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowLedger == true)
                    {
                        try
                        {
                            if (iIDFromEditWindow > 10)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Ldger[" + txtledgerName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                             else
                              MessageBox.Show("Default Ledger [" + txtledgerName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        //For Casing
        private void txtledgerName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerName, true);
        }
        private void txtledgerName_Leave(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                txtledgerAliasName.Text = txtledgerName.Text;

                //if (txtledgerName.Text.Length > 7)
                //{
                //    if (txtledgerAliasName.Text.Trim() == "")
                //        txtledgerAliasName.AppendText(txtledgerName.Text.Substring(0, 7));
                //}
                //else
                //{
                //    if (txtledgerAliasName.Text.Trim() == "")
                //        txtledgerAliasName.AppendText(txtledgerName.Text);
                //}
                
                Comm.ControlEnterLeave(txtledgerName);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  append Ledger Alias Name to Ledger Alias...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtledgerAliasName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerAliasName, true);
        }
        private void txtledgerAliasName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerAliasName);
        }
        private void cboGroup_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboGroup, true);
        }
        private void cboGroup_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboGroup);
        }
        private void cboGsttyp_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboGsttyp, true);
        }
        private void cboGsttyp_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboGsttyp);
        }
        private void trvwParentGroup_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentGroup, true);
        }
        private void trvwParentGroup_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentGroup);
        }
        private void txtledgerAdd_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerAdd);
        }
        private void txtledgerAdd_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerAdd, true);
        }
        private void txtledgerMob_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerMob, true);
        }
        private void txtledgerMob_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerMob);
        }
        private void txtledgerEmail_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerEmail, true);
        }
        private void txtledgerEmail_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerEmail, false, false);
        }
        private void cboState_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboState, true);
        }
        private void cboState_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboState);
        }
        private void txtledgerTaxreg_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerTaxreg, true);
        }
        private void txtledgerTaxreg_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerTaxreg);
        }
        private void txtArea_Enter(object sender, EventArgs e)
        {
            txtArea.TextChanged -= txtArea_TextChanged;
            Comm.ControlEnterLeave(txtArea, true);
            txtArea.TextChanged += txtArea_TextChanged;
        }
        private void txtArea_Leave(object sender, EventArgs e)
        {
            txtArea.TextChanged -= txtArea_TextChanged;
            Comm.ControlEnterLeave(txtArea);
            txtArea.TextChanged += txtArea_TextChanged;
        }
        private void txtAgent_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgent, true);
        }
        private void txtAgent_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgent);
        }
        private void txtledgerOpbal_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtledgerOpbal.Text))
                txtledgerOpbal.Text = "0";
            Comm.ControlEnterLeave(txtledgerOpbal);
            txtledgerOpbal.Text = FormatValue(Convert.ToDouble(txtledgerOpbal.Text), true, "");
        }
        private void txtledgerOpbal_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerOpbal, true);
        }
        private void cboLedgerOpTyp_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboLedgerOpTyp, true);
        }
        private void cboLedgerOpTyp_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboLedgerOpTyp);

        }
        private void txtledgerDiffopbal_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtledgerDiffopbal, true);
        }
        private void txtledgerDiffopbal_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtledgerDiffopbal.Text))
                txtledgerDiffopbal.Text = "0";
            Comm.ControlEnterLeave(txtledgerDiffopbal, true);
            txtledgerOpbal.Text = FormatValue(Convert.ToDouble(txtledgerDiffopbal.Text), true, "");
        }
        private void txtdiscPer_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtdiscPer, true);
        }
        private void txtdiscPer_Leave(object sender, EventArgs e)
        {
            if(txtdiscPer.Text=="")
                txtdiscPer.Text="0";
            else if (txtdiscPer.Text.TrimEnd().TrimStart() == ".")
                txtdiscPer.Text = ".0";
            Comm.ControlEnterLeave(txtdiscPer,false,false);
            txtdiscPer.Text = FormatValue(Convert.ToDouble(txtdiscPer.Text), true, "#.00");
        }
        private void cboPriceList_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboPriceList, true);
        }
        private void cboPriceList_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboPriceList, false, false);
        }

        //Tab Focus
        private void txtledgerName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerAliasName.Focus();
            }
        }
        private void txtledgerAliasName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboGroup.Focus();
            }
        }
        private void cboGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboGsttyp.Focus();
            }
        }
        private void cboGsttyp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                trvwParentGroup.Focus();
            }
        }
        private void trvwParentGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerAdd.Focus();
            }
        }
        private void txtledgerAdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerMob.Focus();
            }
        }
        private void txtledgerMob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerEmail.Focus();
            }

        }
        private void txtledgerEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboState.Focus();
            }
        }
        private void txtArea_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                if(txtAgent.Visible==true)
                    txtAgent.Focus();
                else
                txtledgerOpbal.Focus();
            }
        }
        private void cboState_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerTaxreg.Focus();
            }
        }
        private void txtledgerTaxreg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                if(txtArea.Visible==true)
                txtArea.Focus();
                else if (txtAgent.Visible == true)
                    txtAgent.Focus();
                else
                    txtledgerOpbal.Focus();
            }
        }
        private void txtledgerDiffopbal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.Focus();
            }
        }
        private void txtledgerOpbal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboLedgerOpTyp.Focus();
            }
        }
        private void cboLedgerOpTyp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerDiffopbal.Focus();
            }
        }
        private void txtAgent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtledgerOpbal.Focus();
            }
        }

        private void txtledgerName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtledgerName.Text))
            {
                txtledgerAliasName.Clear();
            }
        }
        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            //if (this.ActiveControl.Name != txtArea.Name)
            if (this.ActiveControl.Name != "txtArea")
                return;
            if (txtArea.Text != "")
            {
                string sQuery = "SELECT ISNULL(Area1.Area,'') + ISNULL(Area2.Area,'') As AnyWhere,Area1.Area As [Sub Area],Area2.Area As [Area],Area1.AreaID FROM tblarea Area1 FULL JOIN tblarea Area2 ON Area2.AreaID=Area1.ParentID  WHERE Area1.TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromAreaSearch, sQuery, "AnyWhere|ISNULL(Area1.Area,'')|ISNULL(Area2.Area,'')", txtArea.Location.X + 477, txtArea.Location.Y + 100, 2, 0, txtArea.Text, 3, 0, "ORDER BY Area ASC", 0, 0, "Area Search ...", 0, "200,100,0", true, "frmAreaMaster").ShowDialog();
                 SendKeys.Send("{Tab}");
                SendKeys.Send("+{DOWN}");
            }
        }
        private void txtAgent_TextChanged(object sender, EventArgs e)
        {
            if (this.ActiveControl.Name != "txtAgent")
                return;
            if (txtAgent.Text != "")
            {
                string sQuery = "SELECT ISNULL(AgentName,'') + ISNULL(AgentCode,'') +  ISNULL(ADDRESS,'') + ISNULL(Area,'') As AnyWhere,AgentName As [Agent Name],AgentCode As [Agent Code],ADDRESS As [ADDRESS],Area,AgentID FROM tblAgent WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromAgentSearch, sQuery, "AnyWhere|ISNULL(AgentName,'')|ISNULL(AgentCode,'')|ISNULL(ADDRESS,'')|ISNULL(Area,'')", txtAgent.Location.X + 477, txtAgent.Location.Y + 100, 4, 0, txtAgent.Text, 5, 0, "ORDER BY AgentName ASC", 0, 0, "Agent Search ...", 0, "100,150,150,100,0", true, "frmAgent").ShowDialog();
                        SendKeys.Send("{Tab}");
            }
        }
        private void txtledgerOpbal_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtledgerOpbal.Text))
            {
                decimal opbal;
                if (txtledgerOpbal.Text == ".")
                {
                    string bal = "0.";
                    opbal = Convert.ToDecimal(bal);
                }
                else
                    opbal = Convert.ToDecimal(txtledgerOpbal.Text);

                if (opbal > 0)
                    cboLedgerOpTyp.SelectedIndex = 1;
                else
                    cboLedgerOpTyp.SelectedIndex = 0;
            }
            else
                cboLedgerOpTyp.SelectedIndex = 0;
        }
        private void txtledgerOpbal_KeyPress(object sender, KeyPressEventArgs e)
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
        private void txtledgerMob_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar.ToString() != "+" && e.KeyChar.ToString() != ",";
        }
        private void cboGroup_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillTreeview();
            SetTreeviewDefaultValue();
            if (cboGroup.SelectedIndex == 2)
            {
                lblAdd.Visible = false;
                txtledgerAdd.Visible = false;
                trvwParentGroup.Size = new Size(260, 243);
            }
            else
            {
                lblAdd.Visible = true;
                txtledgerAdd.Visible = true;
                trvwParentGroup.Size = new Size(260, 115);
            }
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
        private void btnEditLArea_Click(object sender, EventArgs e)
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
        private void btnAddAgent_Click(object sender, EventArgs e)
        {
            try
            {
                frmAgentMaster frmAgent = new frmAgentMaster(0,false,txtAgent);
                frmAgent.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEditAgent_Click(object sender, EventArgs e)
        {
            try
            {
                frmAgentMaster frmAgent = new frmAgentMaster(Convert.ToInt32(txtAgent.Tag), true, txtAgent);
                frmAgent.Show();
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
                MessageBox.Show("Failed to save" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string msgName = "";
            Cursor.Current = Cursors.WaitCursor;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            if (iIDFromEditWindow > 100)
            {
                if (strSUPORCUS == "SUPPLIER")
                    msgName = "Supplier";
                else if (strSUPORCUS == "CUSTOMER")
                    msgName = "Customer";
                else
                    msgName = "Ledger";
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete "+ msgName +"[" + txtledgerName.Text + "] Permanently ?", Global.gblMessageCaption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    try
                    {
                        DeleteData();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to Delete " + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            else
                MessageBox.Show("Default " + msgName + "[" + txtledgerName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Failed to Find" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtledgerName.Text != "")
            {
                if (txtledgerName.Text != strCheck)
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
        private bool IsValidate()//Validate Ledger Name
        {
            bool bValidate = true;
            if (txtledgerName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Ledger Name ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtledgerName.Focus();
            }
            else if (txtledgerName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Ledger Alias Name ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtledgerAliasName.Focus();
            }
            else if(cboGsttyp.Text == "B2B" && string.IsNullOrEmpty(txtledgerTaxreg.Text))
            {
                bValidate = false;
                MessageBox.Show("Please Enter Tax Registration No", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtledgerTaxreg.Focus();
            }
            else
            {
                txtledgerAliasName.Text = txtledgerAliasName.Text.Replace("'", "\"");
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
        //Description :Application Settings 
        private void ApplicationSettings() 
        {
            //Show Customer Area
            if (AppSettings.NeedCustArea == false)
            {
                txtArea.TextChanged -= txtArea_TextChanged;
                txtArea.Visible = false;
                txtArea.TextChanged += txtArea_TextChanged;
                lblArea.Visible = false;
                btnAddArea.Visible = false;
                btnEditLArea.Visible = false;

                lblAgent.Location = new Point(295, 203);
                txtAgent.Location = new Point(295, 230);
                btnAddAgent.Location = new Point(510, 205);
                btnEditAgent.Location = new Point(540, 205);
                gboxOpDetails.Location = new Point(295, 260);
            }
            //Show Agent
            if (AppSettings.NeedAgent == false)
            {
                txtAgent.TextChanged -= txtAgent_TextChanged;
                txtAgent.Visible = false;
                txtAgent.TextChanged += txtAgent_TextChanged;
                lblAgent.Visible = false;
                btnAddAgent.Visible = false;
                btnEditAgent.Visible = false;

                gboxOpDetails.Location = new Point(295, 260);
            }
            if (AppSettings.NeedCustArea == false && AppSettings.NeedAgent == false)
            {
                gboxOpDetails.Location = new Point(295, 215);
            }
        }
        //Description :Fill Data in PriceList Combobox based on Settings
        private void LoadPriceList()
        {
            DataTable dtPriceList = new DataTable();
            dtPriceList.Clear();

            dtPriceList.Columns.Add("PLID");
            dtPriceList.Columns.Add("PriceListName");

            DataRow dRow1 = dtPriceList.NewRow();
            dRow1["PLID"] = "0";
            dRow1["PriceListName"] = "<None>";
            dtPriceList.Rows.Add(dRow1);

            if (AppSettings.IsActiveSRate1 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate1Name.Trim()))
                    SR1 = "SRate 1";
                else
                    SR1 = AppSettings.SRate1Name;

                DataRow dRow2 = dtPriceList.NewRow();
                dRow2["PLID"] = "1";
                dRow2["PriceListName"] = SR1;
                dtPriceList.Rows.Add(dRow2);
            }
            if (AppSettings.IsActiveSRate2 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate2Name.Trim()))
                    SR2 = "SRate 2";
                else
                    SR2 = AppSettings.SRate2Name;

                DataRow dRow3 = dtPriceList.NewRow();
                dRow3["PLID"] = "2";
                dRow3["PriceListName"] = SR2;
                dtPriceList.Rows.Add(dRow3);
            }
            if (AppSettings.IsActiveSRate3 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate3Name.Trim()))
                    SR3 = "SRate 3";
                else
                    SR3 = AppSettings.SRate3Name;

                DataRow dRow4 = dtPriceList.NewRow();
                dRow4["PLID"] = "3";
                dRow4["PriceListName"] = SR3;
                dtPriceList.Rows.Add(dRow4);
            }
            if (AppSettings.IsActiveSRate4 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate4Name.Trim()))
                    SR4 = "SRate 4";
                else
                    SR4 = AppSettings.SRate4Name;

                DataRow dRow5 = dtPriceList.NewRow();
                dRow5["PLID"] = "4";
                dRow5["PriceListName"] = SR4;
                dtPriceList.Rows.Add(dRow5);
            }
            if (AppSettings.IsActiveSRate5 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate5Name.Trim()))
                        SR5 = "SRate 5";
                else
                    SR5 = AppSettings.SRate5Name;

                DataRow dRow6 = dtPriceList.NewRow();
                dRow6["PLID"] = "5";
                dRow6["PriceListName"] = SR5;
                dtPriceList.Rows.Add(dRow6);
            }

            if (string.IsNullOrEmpty(AppSettings.MRPName.Trim()))
                MRP = "MRP";
            else
                MRP = AppSettings.MRPName;

            DataRow dRow7 = dtPriceList.NewRow();
            dRow7["PLID"] = "6";
            dRow7["PriceListName"] = MRP;
            dtPriceList.Rows.Add(dRow7);

            cboPriceList.DataSource = dtPriceList;
            cboPriceList.DisplayMember = "PriceListName";
            cboPriceList.ValueMember = "PLID";

            cboPriceList.SelectedIndex = 0;
        }
        //Description :Fill State in Combobox
        private void FillStates(int iSelID = 0)
        {
            DataTable dtState = new DataTable();
            dtState = Comm.fnGetData("SELECT StateCode,State,StateId FROM tblStates where TenantID =" + Global.gblTenantID + "").Tables[0];
            if (dtState.Rows.Count > 0)
            {
                Comm.LoadControl(cboState, dtState, "", false, false, "State", "StateId");
                if (iSelID != 0)
                {
                    cboState.SelectedValue = iSelID;
                    foreach (System.Data.DataRow row in dtState.Rows)
                    {
                        if (Convert.ToDecimal(row["StateId"].ToString()) == iSelID)
                        {
                            lblStateCode.Text = row["StateCode"].ToString();
                            dStateID = Convert.ToDecimal(row["StateId"].ToString());
                        }
                    }
                }
            }
        }
        //Description :Fill Parent Account Group Tree View
        private void FillTreeview()
        {
            DataTable dtTreeView = new DataTable();
            TreeNode parentNode;
            strGroupName = Convert.ToString(cboGroup.SelectedItem);
            if (strGroupName == "LEDGER")
            {
                dtTreeView = Comm.fnGetData("SELECT AccountGroupID,AccountGroup,Nature,MaintainBudget,SortOrder,ParentID,HID,ACTIVESTATUS,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblAccountGroup WHERE  ParentID=0 AND UPPER(AccountGroup) NOT IN ('CUSTOMER','SUPPLIER') AND TenantID=" + Global.gblTenantID + "").Tables[0];
                trvwParentGroup.Nodes.Clear();
                foreach (DataRow dr in dtTreeView.Rows)
                {
                    parentNode = trvwParentGroup.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                    if (Convert.ToInt32(dr["AccountGroupID"].ToString()) != 0)
                    {
                        PopulateTreeView(Convert.ToInt32(dr["AccountGroupID"].ToString()), parentNode);
                        dAccGroupID = Convert.ToDecimal(dr["AccountGroupID"]);
                    }
                }
                trvwParentGroup.ExpandAll();
            }
            else
            {
                dtTreeView = Comm.fnGetData("EXEC Usp_AccGrpGetNature '" + strGroupName + "'," + Global.gblTenantID + "").Tables[0];
                trvwParentGroup.Nodes.Clear();
                foreach (DataRow dr in dtTreeView.Rows)
                {
                    parentNode = trvwParentGroup.Nodes.Add(dr["AccountGroupID"].ToString(), dr["Nature"].ToString());
                    if (Convert.ToInt32(dr["AccountGroupID"].ToString()) != 0)
                    {
                        PopulateTreeView(Convert.ToInt32(dr["AccountGroupID"].ToString()), parentNode);
                        dAccGroupID = Convert.ToDecimal(dr["AccountGroupID"]);
                    }
                }
                trvwParentGroup.ExpandAll();
            }
        }
        //Description :Fill Child Account Group Tree View
        private void PopulateTreeView(int parentId, TreeNode parentNode)
        {
            DataTable dtgetData = new DataTable();
            TreeNode childNode;
            if (strGroupName == "LEDGER")
            {
                dtgetData = Comm.fnGetData("SELECT AccountGroupID,AccountGroup,Nature,MaintainBudget,SortOrder,ParentID,HID,ACTIVESTATUS,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblAccountGroup WHERE  UPPER(AccountGroup) NOT IN ('CUSTOMER','SUPPLIER') AND ParentID=" + parentId).Tables[0];
                foreach (DataRow dr in dtgetData.Rows)
                {
                    if (parentNode == null)
                    {
                        childNode = trvwParentGroup.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                    }
                    else
                    {
                        parentNode.Tag = dr["ParentID"].ToString();
                        childNode = parentNode.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                    }
                    PopulateTreeView(Convert.ToInt32(dr["AccountGroupID"].ToString()), childNode);
                }
            }
            else
            {
                dtgetData = Comm.fnGetData("EXEC UspAccGrpGetNatureChild " + parentId + ",'" + strGroupName + "'").Tables[0];
                foreach (DataRow dr in dtgetData.Rows)
                {
                    if (parentNode == null)
                    {
                        childNode = trvwParentGroup.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                    }
                    else
                    {
                        parentNode.Tag = dr["ParentID"].ToString();
                        childNode = parentNode.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                    }
                    PopulateTreeView(Convert.ToInt32(dr["AccountGroupID"].ToString()), childNode);
                }
            }
        }
        //Description :Get Parent Nodes
        private string GetParentNodes(TreeNode node_)
        {
            string sNodeIds = "";
            TreeNode[] nodes_ = new TreeNode[node_.Level + 1];
            nodes_[0] = node_;
            sNodeIds = node_.Name;
            for (int i = 1; i < nodes_.Length; i++)
            {
                if (nodes_[i - 1] != null)
                {
                    nodes_[i] = nodes_[i - 1].Parent;
                    sNodeIds = sNodeIds + "," + nodes_[i].Name;
                }
            }
            return "," + sNodeIds + ",0,";
        }
        //Description : Call Area Compact search
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
                        return true;
                    }
                }
            }
            else
                return false;
        }
        //Description : Call Agent Compact search
        private Boolean GetFromAgentSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtAgent = new DataTable();

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
                        AgentInfo.AgentID = Convert.ToInt32(sCompSearchData[0].ToString());
                        AgentInfo.TenantID = Global.gblTenantID;
                        dtAgent = clsAgent.GetAgentMaster(AgentInfo);
                        if (dtAgent.Rows.Count > 0)
                        {
                            this.txtAgent.TextChanged -= this.txtAgent_TextChanged;
                            txtAgent.Text = dtAgent.Rows[0]["AgentName"].ToString();
                            this.txtAgent.TextChanged += this.txtAgent_TextChanged;
                            txtAgent.Tag = dtAgent.Rows[0]["AgentID"].ToString();
                        }
                        return true;
                    }
                    else
                    {
                        this.txtAgent.TextChanged -= this.txtAgent_TextChanged;
                        txtAgent.Text = sCompSearchData[1].ToString();
                        this.txtAgent.TextChanged += this.txtAgent_TextChanged;
                        return true;
                    }
                }
            }
            else
                return false;
        }

        private void txtledgerAliasName_TextChanged(object sender, EventArgs e)
        {

        }

        //Description : Set Default Value when Control is Empty
        private void SetDefaultValue()
        {
            if (string.IsNullOrEmpty(txtArea.Text))
            {
                txtArea.Tag = 1;
                this.txtArea.TextChanged -= this.txtArea_TextChanged;
                txtArea.Text = Comm.fnGetData("Select Area From tblArea Where AreaID = '" + txtArea.Tag + "'").Tables[0].Rows[0][0].ToString();
                this.txtArea.TextChanged += this.txtArea_TextChanged;
            }
            if (string.IsNullOrEmpty(txtAgent.Text))
            {
                txtAgent.Tag = 1;
                this.txtAgent.TextChanged -= this.txtAgent_TextChanged;
                txtAgent.Text = Comm.fnGetData("Select AgentName From tblAgent Where AgentID = '" + txtAgent.Tag + "'").Tables[0].Rows[0][0].ToString();
                this.txtAgent.TextChanged += this.txtAgent_TextChanged;
            }
        }
        //Description : Set Treeview Default Value when Control is Empty
        private void SetTreeviewDefaultValue()
        {
            if (cboGroup.SelectedIndex == 1)
            {
                    dGroupID = 10;
                    strAccGroup = Comm.fnGetData("Select AccountGroup From tblaccountgroup Where AccountGroupID = '" + dGroupID + "'").Tables[0].Rows[0][0].ToString();
            }
            else if (cboGroup.SelectedIndex == 2)
            {
                if (itvwParentID <= 1)
                {
                    if (dAccGroupDefaultEmployee == 20)
                    {
                        dGroupID = 20;
                        strAccGroup = Comm.fnGetData("Select AccountGroup From tblaccountgroup Where AccountGroupID = '" + dGroupID + "'").Tables[0].Rows[0][0].ToString();
                    }
                    else
                    {
                        dGroupID = 1;
                    }
                }
                else
                    dGroupID = itvwParentID;
                strAccGroup = Comm.fnGetData("Select AccountGroup From tblaccountgroup Where AccountGroupID = '" + dGroupID + "'").Tables[0].Rows[0][0].ToString();
               
            }
            else
            {
                dGroupID = 11;
                strAccGroup = Comm.fnGetData("Select AccountGroup From tblaccountgroup Where AccountGroupID = '" + dGroupID + "'").Tables[0].Rows[0][0].ToString();
            }
            trvwParentGroup.SelectedNode = Comm.GetNodeByText(trvwParentGroup.Nodes, strAccGroup);
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataTable dtLoad = new DataTable();
                GetLedgerInfo.LID = Convert.ToDecimal(iSelectedID);
                GetLedgerInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                dtLoad = clsled.GetLedgerDetail(GetLedgerInfo);

                if (dtLoad.Rows.Count > 0)
                {
                    txtledgerName.Text = dtLoad.Rows[0]["LName"].ToString();
                    strCheck = dtLoad.Rows[0]["LName"].ToString();
                    txtledgerAliasName.Text = dtLoad.Rows[0]["LAliasName"].ToString();
                    txtledgerAdd.Text = dtLoad.Rows[0]["Address"].ToString();
                    txtledgerMob.Text = dtLoad.Rows[0]["MobileNo"].ToString();
                    txtledgerEmail.Text = dtLoad.Rows[0]["Email"].ToString();
                    txtledgerTaxreg.Text = dtLoad.Rows[0]["TaxNo"].ToString();
                    dStateID = Convert.ToInt32(dtLoad.Rows[0]["StateID"].ToString());
                    if (dStateID > 0)
                        FillStates(Convert.ToInt32(dtLoad.Rows[0]["StateID"].ToString()));
                    else
                    {
                        FillStates();
                        cboState.SelectedIndex = -1;
                    }

                    if (Convert.ToInt32(dtLoad.Rows[0]["ActiveStatus"].ToString()) == 1)
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                
                    txtdiscPer.Text = dtLoad.Rows[0]["DiscPer"].ToString();
                    int plid = Convert.ToInt32(dtLoad.Rows[0]["PLID"].ToString());
                    LoadPriceList();
                    cboPriceList.SelectedValue = plid;
                    txtledgerOpbal.Text = dtLoad.Rows[0]["OpBalance"].ToString();
                    cboGsttyp.SelectedItem = dtLoad.Rows[0]["GSTType"].ToString();
                    cboGroup.SelectedItem = dtLoad.Rows[0]["GroupName"].ToString();
                    cboLedgerOpTyp.SelectedItem = dtLoad.Rows[0]["Type"].ToString();
                    itvwParentID = Convert.ToInt32(dtLoad.Rows[0]["AccountGroupID"].ToString());
                    strAccGroup = dtLoad.Rows[0]["AccountGroup"].ToString();
                   
                    txtArea.TextChanged -= txtArea_TextChanged;
                    txtArea.Text = dtLoad.Rows[0]["Area"].ToString();
                    txtArea.TextChanged += txtArea_TextChanged;
                    txtArea.Tag = dtLoad.Rows[0]["AreaID"].ToString();
                    txtAgent.TextChanged -= txtAgent_TextChanged;
                    txtAgent.Text = dtLoad.Rows[0]["AgentName"].ToString();
                    txtAgent.TextChanged += txtAgent_TextChanged;
                    txtAgent.Tag = dtLoad.Rows[0]["AgentID"].ToString();
                    iAction = 1;
                }
                Cursor.Current = Cursors.Default; ;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load  Data from Edit" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                 string[] strResult;
                 string strRet = "";
                 int iActive = 0;
                if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActive = 1;

                TreeNode tn = trvwParentGroup.SelectedNode;
                 string strtnds = ",0,";
                 if (tn != null)
                 {
                      itvwParentID = Convert.ToInt32(tn.Name);
                      strtnds = GetParentNodes(tn);
                 }
                if (iAction == 0)
                {
                    LedgerInfo.LID = Comm.gfnGetNextSerialNo("tblLedger", "LID");
                    if (LedgerInfo.LID < 101)
                        LedgerInfo.LID = 101;
                }
                else
                    LedgerInfo.LID = iIDFromEditWindow;
                LedgerInfo.LName = txtledgerName.Text;
                LedgerInfo.LAliasName = txtledgerAliasName.Text;
                LedgerInfo.GroupName = Convert.ToString(cboGroup.SelectedItem);
                LedgerInfo.GSTType = Convert.ToString(cboGsttyp.SelectedItem);
                LedgerInfo.Address = txtledgerAdd.Text;
                LedgerInfo.MobileNo = txtledgerMob.Text;
                LedgerInfo.Email = txtledgerEmail.Text;
                dStateID = Convert.ToInt32(cboState.SelectedValue);
                LedgerInfo.StateID = dStateID;
                LedgerInfo.TaxNo = txtledgerTaxreg.Text;
                if (string.IsNullOrEmpty(txtdiscPer.Text))
                    txtdiscPer.Text = "0";
                LedgerInfo.DiscPer = Convert.ToDecimal(txtdiscPer.Text);
                LedgerInfo.PLID = Convert.ToDecimal(cboPriceList.SelectedValue);
                if (string.IsNullOrEmpty(txtledgerOpbal.Text))
                    txtledgerOpbal.Text = "0";
                LedgerInfo.OpBalance = Convert.ToDecimal(txtledgerOpbal.Text);
                LedgerInfo.Type = Convert.ToString(cboLedgerOpTyp.SelectedItem);
                LedgerInfo.EntryDate = DateTime.Today;
                LedgerInfo.DOB = DateTime.Today;

                tn = trvwParentGroup.SelectedNode;
                if (tn != null)
                {
                    itvwParentID = Convert.ToInt32(tn.Name);
                    strtnds = GetParentNodes(tn);
                }
                if (tn != null)
                    LedgerInfo.AccountGroupID = itvwParentID;
                else
                    LedgerInfo.AccountGroupID = 0;
                LedgerInfo.ActiveStatus = iActive;
                string name = Environment.MachineName;
                LedgerInfo.SystemName = name;
                LedgerInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                LedgerInfo.LastUpdateDate = DateTime.Today;
                LedgerInfo.LastUpdateTime = DateTime.Now;
                LedgerInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                if (AppSettings.NeedCustArea == true)
                {
                        SetDefaultValue();
                        LedgerInfo.Area = txtArea.Text;
                        LedgerInfo.AreaID = Convert.ToDecimal(txtArea.Tag);
                }
                else
                {
                    SetDefaultValue();
                }
                if (AppSettings.NeedAgent == true)
                {
                    LedgerInfo.AgentID = Convert.ToDecimal(txtAgent.Tag);
                }
                else
                    LedgerInfo.AgentID = 1;
                strRet = clsled.InsertUpdateDeleteLedger(LedgerInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                           if (strResult[1].ToString().Contains("IX_tblLedger"))
                           {
                              MessageBox.Show("Duplicate Entry, User has restricted to enter duplicate values in the Ledger alias name (" + txtledgerAliasName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                              txtledgerAliasName.Focus();
                              txtledgerAliasName.SelectAll();
                           }
                           else
                           {
                              MessageBox.Show("Duplicate Entry, User has restricted to enter duplicate values in the Ledger name(" + txtledgerName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                              txtledgerName.Focus();
                                //txtledgerName.SelectAll();
                                txtledgerName.SelectionStart = txtledgerName.Text.ToString().Length;

                            }
                        }
                        else
                             MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        if (CtrlPassed is TextBox)
                        {
                            string ledname = txtledgerName.Text;
                            decimal lid = LedgerInfo.LID;
                            //CtrlPassed.Text = txtledgerName.Text;
                            //CtrlPassed.Tag = LedgerInfo.LID;
                            //CtrlPassed.Focus();

                            //CtrlPassed_Textchanged event is triggered which opens subwindow as dialog.
                            //So the ledger window will not be closed after that.
                            //First close the ledger window, then assign the values.
                            this.Close();
                            CtrlPassed.Focus();
                            CtrlPassed.Tag = lid;
                            CtrlPassed.Text = ledname;

                        }
                        else if (CtrlPassed is ComboBox)
                        {
                            DataTable dtBrand = Comm.fnGetData("SELECT LID,LName FROM tblLedger  WHERE TenantID = " + Global.gblTenantID + " ORDER BY LName Asc").Tables[0];
                            ((ComboBox)CtrlPassed).DataSource = dtBrand;
                            ((ComboBox)CtrlPassed).DisplayMember = "LName";
                            ((ComboBox)CtrlPassed).ValueMember = "LID";
                            ((ComboBox)CtrlPassed).SelectedValue = LedgerInfo.LID;
                            ((ComboBox)CtrlPassed).Tag = LedgerInfo.LID;

                            CtrlPassed.Focus();
                            this.Close();
                        }
                    }
                    else
                    {
                        ClearAll();
                        FillTreeview();
                        Settreeview();
                        if (bFromEditWindowLedger == true)
                          {
                              this.Close();
                          }
                    }
                    Comm.MessageboxToasted("Ledger", "Ledger saved successfully");
                }
            }
        }
        //Description :  Delete Data from Ledger table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            int iActive = 0;
            if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActive = 1;
            iAction = 2;
            DataTable dtUspLedger = new DataTable();
            LedgerInfo.LID = iIDFromEditWindow;
            LedgerInfo.LName = txtledgerName.Text;
            LedgerInfo.LAliasName = txtledgerAliasName.Text;
            LedgerInfo.GroupName = Convert.ToString(cboGroup.SelectedItem);
            LedgerInfo.GSTType = Convert.ToString(cboGsttyp.SelectedItem);
            LedgerInfo.Address = txtledgerAdd.Text;
            LedgerInfo.MobileNo = txtledgerMob.Text;
            LedgerInfo.Email = txtledgerEmail.Text;
            LedgerInfo.StateID = Convert.ToInt32(cboState.SelectedValue);
            LedgerInfo.TaxNo = txtledgerTaxreg.Text;
            LedgerInfo.DiscPer = Convert.ToDecimal(txtdiscPer.Text);
            LedgerInfo.PLID = Convert.ToDecimal(cboPriceList.SelectedValue);
            LedgerInfo.OpBalance = Convert.ToDecimal(txtledgerOpbal.Text);
            LedgerInfo.Type = Convert.ToString(cboLedgerOpTyp.SelectedItem);
            LedgerInfo.SystemName = Environment.MachineName;
            LedgerInfo.UserID = Global.gblUserID;
            LedgerInfo.LastUpdateDate = DateTime.Today;
            LedgerInfo.LastUpdateTime = DateTime.Now;
            LedgerInfo.TenantID = Global.gblTenantID;
            LedgerInfo.ActiveStatus = iActive;
            LedgerInfo.DOB = DateTime.Today;
            LedgerInfo.EntryDate = DateTime.Today;
            if (AppSettings.NeedCustArea==true)
            {
                LedgerInfo.Area = txtArea.Text;
                LedgerInfo.AreaID = Convert.ToDecimal(txtArea.Tag);
            }
            else
            {
                LedgerInfo.Area = "";
                LedgerInfo.AreaID = 1;
            }
            if (AppSettings.NeedAgent==true)
                LedgerInfo.AgentID = Convert.ToDecimal(txtAgent.Tag);
            else
                LedgerInfo.AgentID = 1;

            strRet = clsled.InsertUpdateDeleteLedger(LedgerInfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        if (strSUPORCUS == "SUPPLIER")
                            MessageBox.Show("Hey! There are entries associated with this Supplier(" + txtledgerName.Text + ").Please Check", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else if (strSUPORCUS == "CUSTOMER")
                            MessageBox.Show("Hey! There are entries associated with this Customer(" + txtledgerName.Text + ").Please Check", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Hey! There are entries associated with this Ledger(" + txtledgerName.Text + ").Please Check", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (bFromEditWindowLedger == true)
            {
                this.Close();
            }
        }
        //Description :  Set selection on treeview
        private void Settreeview()
        {
            if (strSUPORCUS == "SUPPLIER")
            {
                cboGroup.SelectedIndex = 0;
                trvwParentGroup.Focus();
                lblHeading.Text = "Supplier";
                lblName.Text = "Supplier Name:";
            }
            else if (strSUPORCUS == "CUSTOMER")
            {
                cboGroup.SelectedIndex = 1;
                trvwParentGroup.Focus();
                lblHeading.Text = "Customer";
                lblName.Text = "Customer Name:";
            }
            else
            {
                cboGroup.SelectedIndex = 2;
                SetTreeviewDefaultValue();
                lblHeading.Text = "Ledger";
                lblName.Text = "Ledger Name:";
            }

            if (cboGroup.SelectedIndex == 2)
            {
                txtledgerAdd.Visible = false;
                lblAdd.Visible = false;
                trvwParentGroup.Size = new Size(260, 243);
            }
            else
            {
                txtledgerAdd.Visible = true;
                lblAdd.Visible = true;
                trvwParentGroup.Size = new Size(260, 115);
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtledgerName.Text = "";
            txtledgerAliasName.Text = "";
            txtledgerMob.Text = "";
            txtledgerEmail.Text = "";
            txtledgerTaxreg.Text = "";
            txtledgerAdd.Text = "";
            txtledgerDiffopbal.Text = "0";
            txtledgerOpbal.Text = "0";
            txtledgerName.Focus();
            trvwParentGroup.Nodes.Clear();
            dGroupID = 0;
            itvwParentID = 0;
            cboGsttyp.SelectedIndex = 1;
            cboLedgerOpTyp.SelectedIndex = 0;
            if (strSUPORCUS == "SUPPLIER")
            {
                cboGroup.SelectedIndex = 0;
                trvwParentGroup.Focus();
                lblHeading.Text = "Supplier";
                lblName.Text = "Supplier Name:";
                cboGsttyp.SelectedIndex = 0;
            }
            else if (strSUPORCUS == "CUSTOMER")
            {
                cboGroup.SelectedIndex = 1;
                trvwParentGroup.Focus();
                lblHeading.Text = "Customer";
                lblName.Text = "Customer Name:";
                cboGsttyp.SelectedIndex = 1;
            }
            else
            {
                cboGroup.SelectedIndex = 2;
                SetTreeviewDefaultValue();
                lblHeading.Text = "Ledger";
                lblName.Text = "Ledger Name:";
                cboGsttyp.SelectedIndex = 1;
            }
            Settreeview();

            txtArea.TextChanged -= txtArea_TextChanged;
            txtArea.Text = "";
            txtArea.TextChanged += txtArea_TextChanged;
            txtArea.Tag = 1;
            txtAgent.TextChanged -= txtAgent_TextChanged;
            txtAgent.Text = "";
            txtAgent.TextChanged += txtAgent_TextChanged;
            txtAgent.Tag = 1;
            txtdiscPer.Text = "0";
            cboPriceList.SelectedIndex = 0;
            SetDefaultValue();
            string sStateCode = AppSettings.StateCode;
            cboState.SelectedValue = sStateCode;
        }
        #endregion
    }
}

