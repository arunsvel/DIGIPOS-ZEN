using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DigiposZen.InventorBL.Accounts;
using DigiposZen.InventorBL.Master;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;

namespace DigiposZen
{
    // ======================================================== >>
    // Description:Tax Mode Creation
    // Developed By:Anjitha K K
    // Completed Date & Time: 06-Jan-2022 05:45 PM
    // Last Edited By:
    // Last Edited Date & Time:
    // ======================================================== >>

    public partial class frmAccountGroup : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmAccountGroup(int iAccGrpID = 0, bool bFromEdit = false)
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

            iIDFromEditWindow = iAccGrpID;
            bFromEditWindowAccGp = bFromEdit;
            this.BackColor = Global.gblFormBorderColor;
            if (iAccGrpID != 0)
            {
                FillTreeview();
                LoadData(iAccGrpID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtAccountGroup.Focus();
            txtAccountGroup.SelectAll();
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetAccountGroupInfo GetAccGroup = new UspGetAccountGroupInfo();
        UspAccountGroupInsertInfo AccGroupInfo = new UspAccountGroupInsertInfo();
        clsAccountGroup clsAccountGroup = new clsAccountGroup();
        clsMaster clsmast = new clsMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iAction = 0;
        int iIDFromEditWindow;
        int iActive = 0;
        int iActivemaintainbudget = 0;
        int itvwParentID;
        Control ctrl;
        bool bFromEditWindowAccGp;
        bool bDirectDelete = false;
        string strCheck;
        string strNature;
        string strSelectNodeName = "";
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
        private void txtAccountGroup_Click(object sender, EventArgs e)
        {
            toolAccGroup.SetToolTip(txtAccountGroup, "Account Group to show in print and specified area");
        }
        private void txtSortOrder_Click(object sender, EventArgs e)
        {
            toolAccGroup.SetToolTip(txtSortOrder, "Sort Order to show in print and specified area");
        }

        private void frmAccountGroup_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (iIDFromEditWindow == 0)
            {
                ClearAll();
                SetDefaultTreeview();
                this.Show();
                Application.DoEvents();
                FillTransSortOrder();
                togglebtnActive.ToggleState = ToggleButtonState.Active;
                togglebtnbudget.ToggleState = ToggleButtonState.Active;
            }
            else
            {
                FillTreeview();
                trvwParentAccGroup.SelectedNode = Comm.GetNodeByText(trvwParentAccGroup.Nodes, txtAccountGroup.Text);
            }
            txtAccountGroup.Select();
            txtAccountGroup.SelectAll();
            Cursor.Current = Cursors.Default;
        }
        private void frmAccountGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtAccountGroup.Text != "")
                    {
                        if (txtAccountGroup.Text != strCheck)
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
                else if(e.KeyCode == Keys.F5)//Save
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
                        if (bFromEditWindowAccGp == true)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            strSelectNodeName = Convert.ToString(trvwParentAccGroup.SelectedNode.Text);
                            int iAccGrpID = GetAccGroupID();
                            if (Convert.ToDecimal(iAccGrpID) > 100)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Account Group[" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Account Group [" + strSelectNodeName + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
        private void txtAccountGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtAccountGroup.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    trvwParentAccGroup.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void trvwParentAccGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtAccountGroup.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    txtSortOrder.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void txtSortOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    trvwParentAccGroup.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    togglebtnbudget.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void togglebtnbudget_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtSortOrder.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    togglebtnActive.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void togglebtnActive_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    togglebtnbudget.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter key press not working properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        private void txtSortOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)//Tab Focus
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                togglebtnbudget.Focus();
            }
        }
        private void togglebtnbudget_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
        private void txtAccountGroup_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAccountGroup, true);
        }
        private void txtAccountGroup_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAccountGroup);
        }
        private void trvwParentAccGroup_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentAccGroup, true);
        }
        private void trvwParentAccGroup_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentAccGroup);
        }
        private void txtSortOrder_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, true);
        }
        private void txtSortOrder_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, false, false);
        }
        private void trvwParentAccGroup_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (btnDelete.Enabled == false)
            {
                DialogResult dlgResult = MessageBox.Show("Do you want to enable edit mode", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult.Equals(DialogResult.Yes))
                {
                    FillNodeData();
                }
            }
            else if (bDirectDelete == true)
            {
                FillNodeData();
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
                strSelectNodeName = Convert.ToString(trvwParentAccGroup.SelectedNode.Text);

                int iAccGrpID = GetAccGroupID();
                if (Convert.ToDecimal(iAccGrpID) > 100)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Account Group[" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Account Group [" + strSelectNodeName + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            if (txtAccountGroup.Text != "")
            {
                if (txtAccountGroup.Text != strCheck)
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

            if (txtAccountGroup.Text.Trim() == "")
            {
                bValidate = false;

                MessageBox.Show("Please enter the Account Group", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                txtAccountGroup.Focus();
            }
            else
            {
                if (txtSortOrder.Text == "")
                    txtSortOrder.Text = "0";

                txtAccountGroup.Text = txtAccountGroup.Text.Replace("'", "\"");
            }

            return bValidate;
        }
        //Description : Set Sort Order in textbox
        private void FillTransSortOrder()
        {
            DataTable dtTransOrder = new DataTable();
            dtTransOrder = Comm.fnGetData("SELECT MAX(ISNULL(SortOrder, 0)) + 1 as SortOrder FROM tblAccountGroup WHERE TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtTransOrder.Rows.Count > 0)
            {
                txtSortOrder.Text = dtTransOrder.Rows[0]["SortOrder"].ToString();
            }
        }
        //Description : Get Parent Nodesw
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
        //Description : Fill Parentin Treeview
        private void FillTreeview()
        {
            DataTable dtTreeView = new DataTable();
            TreeNode parentNode;
            dtTreeView = Comm.fnGetData("SELECT AccountGroupID,AccountGroup,Nature,MaintainBudget,SortOrder,ParentID,HID,ACTIVESTATUS,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblAccountGroup WHERE ParentID=0").Tables[0];
            trvwParentAccGroup.Nodes.Clear();
            if (dtTreeView.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTreeView.Rows)
                {
                    parentNode = trvwParentAccGroup.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                    PopulateTreeView(Convert.ToInt32(dr["AccountGroupID"].ToString()), parentNode);
                }
                trvwParentAccGroup.ExpandAll();
            }
        }
        //Description :Fill Child Node in Treeview
        private void PopulateTreeView(int parentId, TreeNode parentNode)
        {
            DataTable dtgetData = new DataTable();
            dtgetData = Comm.fnGetData("SELECT AccountGroupID,AccountGroup,Nature,MaintainBudget,SortOrder,ParentID,HID,ACTIVESTATUS,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblAccountGroup WHERE ParentID=" + parentId + "").Tables[0];
            TreeNode childNode;
            foreach (DataRow dr in dtgetData.Rows)
            {
                if (parentNode == null)
                {
                    childNode = trvwParentAccGroup.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                }
                else
                {
                    parentNode.Tag = dr["ParentID"].ToString();
                    childNode = parentNode.Nodes.Add(dr["AccountGroupID"].ToString(), dr["AccountGroup"].ToString());
                }
                PopulateTreeView(Convert.ToInt32(dr["AccountGroupID"].ToString()), childNode);
            }
        }
        //Description : To get AccountGroup of Selected Node
        private void SelectNature(decimal ParentId)
        {
            DataTable dtNature = new DataTable();
            dtNature = Comm.fnGetData("SELECT AccountGroup FROM tblAccountGroup WHERE AccountGroupID=" + ParentId + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtNature.Rows.Count > 0)
            {
                strNature = dtNature.Rows[0]["AccountGroup"].ToString();
            }
        }
        //Description :Get Account Group ID When Double Click on Node
        private int GetAccGroupID()
        {
            AccGroupInfo.AccountGroupID = iIDFromEditWindow;
            TreeNode tn = trvwParentAccGroup.SelectedNode;

            if (Convert.ToInt32(tn.Name) == iIDFromEditWindow)
                AccGroupInfo.AccountGroupID = iIDFromEditWindow;
            else
                AccGroupInfo.AccountGroupID = Convert.ToInt32(tn.Name);

            return AccGroupInfo.AccountGroupID;
        }
        //Description : Fill Data when Double Click on Node
        public void FillNodeData()
        {
            TreeNode tn = trvwParentAccGroup.SelectedNode;
            iIDFromEditWindow = Convert.ToInt32(tn.Name);
            bFromEditWindowAccGp = true;
            LoadData(iIDFromEditWindow);
            btnDelete.Enabled = true;
            bDirectDelete = true;
            txtAccountGroup.Focus();
            txtAccountGroup.SelectAll();
        }
        //Description : Set Treeview Value as Default
        private void SetDefaultTreeview()
        {
            FillTreeview();
            decimal dAccGpID = 1;
            string strAccgpName = Comm.fnGetData("Select AccountGroup From tblAccountGroup Where AccountGroupID = '" + dAccGpID + "'").Tables[0].Rows[0][0].ToString();
            trvwParentAccGroup.SelectedNode = Comm.GetNodeByText(trvwParentAccGroup.Nodes, strAccgpName);
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataTable dtLoad = new DataTable();
                GetAccGroup.AccountGroupID = iSelectedID;
                GetAccGroup.TenantID = Convert.ToDecimal(Global.gblTenantID);
                dtLoad = clsAccountGroup.GetAccountGroup(GetAccGroup);
                if (dtLoad.Rows.Count > 0)
                {
                    txtAccountGroup.Text = dtLoad.Rows[0]["AccountGroup"].ToString();
                    strCheck = dtLoad.Rows[0]["AccountGroup"].ToString();
                    txtSortOrder.Text = dtLoad.Rows[0]["SortOrder"].ToString();
                    itvwParentID = Convert.ToInt32(dtLoad.Rows[0]["ParentID"].ToString());

                    if (Convert.ToDecimal(dtLoad.Rows[0]["MaintainBudget"].ToString()) == 1)
                        togglebtnbudget.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        togglebtnbudget.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

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
                string strtnds = ",0,";
                TreeNode tn = trvwParentAccGroup.SelectedNode;

            if (bDirectDelete == true)
            {
                if (btnDelete.Enabled == false)
                {
                    iAction = 0;
                    if (tn != null)
                    {
                        itvwParentID = Convert.ToInt32(tn.Name);
                        strtnds = GetParentNodes(tn);
                    }
                }
                else
                    iAction = 1;
            }

            if (bFromEditWindowAccGp == false)
            {
                if (tn != null)
                {
                    itvwParentID = Convert.ToInt32(tn.Name);
                    strtnds = GetParentNodes(tn);
                }
            }
            

                if (iAction == 0)
                {
                    AccGroupInfo.AccountGroupID = Comm.gfnGetNextSerialNo("tblAccountGroup", "AccountGroupID");
                    if (AccGroupInfo.AccountGroupID < 101)
                        AccGroupInfo.AccountGroupID = 101;
                }
                else
                    AccGroupInfo.AccountGroupID = iIDFromEditWindow;
                AccGroupInfo.AccountGroup = txtAccountGroup.Text;
                
                if (tn != null)
                    AccGroupInfo.ParentID = itvwParentID;
                else
                    AccGroupInfo.ParentID = 0;

                AccGroupInfo.HID = strtnds;

                if (AccGroupInfo.ParentID == 0)
                    strNature = txtAccountGroup.Text;
                else
                    SelectNature(AccGroupInfo.ParentID);

                AccGroupInfo.Nature = strNature;
                DataTable dtcount = new DataTable();
                dtcount = clsmast.GetChildCount(AccGroupInfo.HID);
                if (dtcount.Rows.Count < 9)
                {    
                    AccGroupInfo.SortOrder = Convert.ToDecimal(txtSortOrder.Text);

                    if (togglebtnbudget.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        iActivemaintainbudget = 1;
                    else
                        iActivemaintainbudget = 0;
                    AccGroupInfo.MaintainBudget = iActivemaintainbudget;
                    if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                       iActive = 1;
                    else
                       iActive = 0;
                    AccGroupInfo.ACTIVESTATUS = iActive;
                    AccGroupInfo.SystemName = Global.gblSystemName;
                    AccGroupInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                    AccGroupInfo.LastUpdateDate = DateTime.Today;
                    AccGroupInfo.LastUpdateTime = DateTime.Now;
                    AccGroupInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                    strRet = clsAccountGroup.InsertUpdateDeleteAccountGroup(AccGroupInfo, iAction);
                    if (strRet.Length > 2)
                    {
                       strResult = strRet.Split('|');
                       if (Convert.ToInt32(strResult[0].ToString()) == -1)
                       {
                           if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                           {
                                MessageBox.Show("Duplicate Entry,User has restricted to enter duplicate values in the Account Group(" + txtAccountGroup.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtAccountGroup.Focus();
                                txtAccountGroup.SelectAll();
                           }
                           else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                       }
                       else
                       {
                            ClearAll();
                            SetDefaultTreeview();
                            Comm.MessageboxToasted("Account Group", "Account Group saved successfully");
                            if (bFromEditWindowAccGp == true && bDirectDelete == false)
                            {
                                this.Close();
                            }
                       }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Save! ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            ClearAll();
                            SetDefaultTreeview();
                            Comm.MessageboxToasted("Account Group", "Account Group saved successfully");
                            if (bFromEditWindowAccGp == true && bDirectDelete == false)
                            {
                                this.Close();
                            }
                        }
                    }
                }
                else
                {
                    txtAccountGroup.Focus();
                    txtAccountGroup.SelectAll();
                    MessageBox.Show("Can't allow more than 5 sub AccountGroup ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        //Description :  Delete Data from Unit table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            int ParentCount = 0;

            iAction = 2;
            DataTable dtTaxMode = new DataTable();
            AccGroupInfo.AccountGroupID = iIDFromEditWindow;
            AccGroupInfo.AccountGroup = txtAccountGroup.Text;
            AccGroupInfo.SortOrder = Convert.ToDecimal(txtSortOrder.Text);
            AccGroupInfo.ParentID = 0;
            AccGroupInfo.HID = "";
            if (AccGroupInfo.ParentID == 0)
                strNature = txtAccountGroup.Text;
            else
                SelectNature(AccGroupInfo.ParentID);

            AccGroupInfo.Nature = strNature;
            AccGroupInfo.SystemName = Environment.MachineName;
            AccGroupInfo.UserID = Global.gblUserID;
            AccGroupInfo.LastUpdateDate = DateTime.Today;
            AccGroupInfo.LastUpdateTime = DateTime.Now;
            AccGroupInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            TreeNode tn = trvwParentAccGroup.SelectedNode;
            if (tn != null)
                AccGroupInfo.ParentID = itvwParentID;
            else
                AccGroupInfo.ParentID = 0;
            if (Convert.ToInt32(tn.Name) == iIDFromEditWindow)
            {
                GetAccGroup.AccountGroupID = iIDFromEditWindow;
                GetAccGroup.TenantID = Convert.ToDecimal(Global.gblTenantID);
                AccGroupInfo.AccountGroupID = GetAccGroup.AccountGroupID;
            }
            else
            {
                GetAccGroup.AccountGroupID = Convert.ToInt32(tn.Name);
                GetAccGroup.TenantID = Convert.ToDecimal(Global.gblTenantID);
                AccGroupInfo.AccountGroupID = GetAccGroup.AccountGroupID;

                DataTable dt = clsAccountGroup.GetAccountGroup(GetAccGroup);
                AccGroupInfo.AccountGroup = Convert.ToString(dt.Rows[0]["AccountGroup"]);
                AccGroupInfo.SortOrder = Convert.ToDecimal(dt.Rows[0]["SortOrder"]);
            }
            DataTable dtParent = clsAccountGroup.CheckParentIDExists(GetAccGroup);
            ParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);
            if (ParentCount == 0)
            {
               strRet = clsAccountGroup.InsertUpdateDeleteAccountGroup(AccGroupInfo, iAction);
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
                      MessageBox.Show("Failed to Delete", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
               }
               ClearAll();
               FillTreeview();
               if (bFromEditWindowAccGp == true && bDirectDelete == false)
               {
                    this.Close();
               }
            }
            else
            {
               MessageBox.Show("Can't allow to delete the parent Account Group (" + strSelectNodeName + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtAccountGroup.Clear();
            txtSortOrder.Clear();
            togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            togglebtnbudget.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            FillTransSortOrder();
            trvwParentAccGroup.Nodes.Clear();
            btnDelete.Enabled = false;
            txtAccountGroup.Focus();
        }
        #endregion

        private void frmAccountGroup_Shown(object sender, EventArgs e)
        {
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
        }
    }
}
