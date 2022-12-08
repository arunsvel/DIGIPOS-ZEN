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
    public partial class frmAreaMaster : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        // ======================================================== >>
        // Description:             Area Creation
        // Developed By:            Pramod Philip
        // Completed Date & Time:   13/09/2021 6.30 PM
        // Last Edited By:          Anjitha k k
        // Last Edited Date & Time: 01-March-2022 04:00 PM
        // ======================================================== >>
        public frmAreaMaster(int iAreaID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

                iIDFromEditWindow = iAreaID;
                bFromEditWindowArea = bFromEdit;
                CtrlPassed = Controlpassed;


                this.BackColor = Global.gblFormBorderColor;
                if (iAreaID != 0)
                {
                    FillTreeview();
                    LoadData(iAreaID);
                }
                else
                {
                    btnDelete.Enabled = false;
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtArea.Text = CtrlPassed.Text.ToString();
                }

                txtArea.Focus();
                txtArea.SelectAll();

                if (blnDisableMinimize == true) btnMinimize.Enabled = false;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Area"+"\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES  -------------------------------------------- >>"
        UspAreaMasterInfo AreaInfo = new UspAreaMasterInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsAreaMaster clsArea = new clsAreaMaster();
        clsMaster clsMaster = new clsMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        int itvwParentID;
        Control ctrl;
        string strCheck;
        string strSelectNodeName = "";
        bool bFromEditWindowArea;
        bool bDirectDelete = false;
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
        private void txtArea_Click(object sender, EventArgs e)
        {
            toolTipArea.SetToolTip(txtArea, "Specify the unique Area Name");
        }
        private void txtRemarks_Click(object sender, EventArgs e)
        {
            toolTipArea.SetToolTip(txtRemarks, "Specify the unique Area Location");
        }

        private void frmAreaMaster_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();
                    this.Show();
                    Application.DoEvents();
                    SetDefaultTreeview();
                    txtArea.Focus();
                }
                else
                {
                    txtArea.Select();
                    FillTreeview();
                    trvwParentArea.SelectedNode = Comm.GetNodeByText(trvwParentArea.Nodes, txtArea.Text);
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtArea.Text = CtrlPassed.Text.ToString();
                }

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Parent Area is not possible to load ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void frmAreaMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtArea.Text != "")
                    {
                        if (txtArea.Text != strCheck)
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
                        SaveData();
                    }

                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowArea == true && btnDelete.Enabled == true)
                    {
                        try
                        {
                            strSelectNodeName = Convert.ToString(trvwParentArea.SelectedNode.Text);
                            decimal iAreaID = GetAreaID();
                            if (Convert.ToDecimal(iAreaID) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete the Area [" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (dlgResult.Equals(DialogResult.Yes))
                                    DeleteData();
                            }
                            else
                                MessageBox.Show("Default Area [" + strSelectNodeName + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Short Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        this.SelectNextControl(ctrl, false, false, false, false);
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
            catch (Exception ex )
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    trvwParentArea.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    Comm.ControlEnterLeave(txtRemarks);
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save..."+"\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtArea_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtArea, true);
        }
        private void txtArea_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtArea);
        }
        private void trvwParentArea_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentArea, true);
        }
        private void trvwParentArea_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentArea);
        }
        private void txtRemarks_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRemarks, true);
        }
        private void txtRemarks_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRemarks);
        }
        private void trvwParentArea_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
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
                MessageBox.Show("Failed to save..."+"\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                strSelectNodeName = Convert.ToString(trvwParentArea.SelectedNode.Text);
                decimal iAreaID = GetAreaID();
                if (Convert.ToDecimal(iAreaID) > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete the Area [" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dlgResult.Equals(DialogResult.Yes))
                        DeleteData();
                }
                else
                    MessageBox.Show("Default Area [" + strSelectNodeName + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
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
                frmEdit.ShowDialog();
                this.Visible = false;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Find..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }       
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtArea.Text != "")
            {
                if (txtArea.Text != strCheck)
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
        private bool IsValidate()
        {
            bool bValidate = true;
            if (txtArea.Text.Trim() == "")
            {
                txtArea.Focus();
                bValidate = false;
                MessageBox.Show("Please enter Area Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (txtRemarks.Text == "")
                    txtRemarks.Text = "";
                txtArea.Text = txtArea.Text.Replace("'", "\"");
            }
            return bValidate;
        }
        //Description : Get Parents of Selected Node
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
        //Description :Get and Set Parent Area in Treeview
        private void FillTreeview()
        {
            DataTable dtTreeView = new DataTable();
            TreeNode parentNode;
            dtTreeView = Comm.fnGetData("SELECT AreaID,Area,Remarks,ParentID,HID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblArea WHERE ParentID=0").Tables[0];
            trvwParentArea.Nodes.Clear();
            if (dtTreeView.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTreeView.Rows)
                {
                    parentNode = trvwParentArea.Nodes.Add(dr["AreaID"].ToString(), dr["Area"].ToString());
                    PopulateTreeView(Convert.ToInt32(dr["AreaID"].ToString()), parentNode);
                }
                trvwParentArea.ExpandAll();
            }
        }
        //Description : Get and Set Child Area in Treeview
        private void PopulateTreeView(int parentId, TreeNode parentNode)//Fill ChildNode
        {
            DataTable dtgetData = new DataTable();
            dtgetData = Comm.fnGetData("SELECT AreaID,Area,Remarks,ParentID,HID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblArea WHERE ParentID=" + parentId + "").Tables[0];
            TreeNode childNode;
            foreach (DataRow dr in dtgetData.Rows)
            {
                if (parentNode == null)
                {
                    childNode = trvwParentArea.Nodes.Add(dr["AreaID"].ToString(), dr["Area"].ToString());
                }
                else
                {
                    parentNode.Tag = dr["ParentID"].ToString();
                    childNode = parentNode.Nodes.Add(dr["AreaID"].ToString(), dr["Area"].ToString());
                }
                PopulateTreeView(Convert.ToInt32(dr["AreaID"].ToString()), childNode);
            }
        }
        //Description : Fill Data when Double Click on TreeView Node
        public void FillNodeData()
        {
            TreeNode tn = trvwParentArea.SelectedNode;
            iIDFromEditWindow = Convert.ToInt32(tn.Name);
            bFromEditWindowArea = true;
            LoadData(iIDFromEditWindow);
            btnDelete.Enabled = true;
            bDirectDelete = true;
            txtArea.Focus();
            txtArea.SelectAll();
        }
        //Description : Get CategoryID When Double Click on Node
        private decimal GetAreaID()
        {
            AreaInfo.AreaID = iIDFromEditWindow;
            TreeNode tn = trvwParentArea.SelectedNode;
            if (Convert.ToInt32(tn.Name) == iIDFromEditWindow)
                AreaInfo.AreaID = iIDFromEditWindow;
            else
                AreaInfo.AreaID = Convert.ToInt32(tn.Name);
            return AreaInfo.AreaID;
        }
        //Description : Set Treeview Value as Default
        private void SetDefaultTreeview()
        {
            FillTreeview();
            decimal dAreaID = 1;
            string strAreaName = Comm.fnGetData("Select Area From tblArea Where AreaID = '" + dAreaID + "'").Tables[0].Rows[0][0].ToString();
            trvwParentArea.SelectedNode = Comm.GetNodeByText(trvwParentArea.Nodes, strAreaName);
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                DataTable dtLoad = new DataTable();
                AreaInfo.AreaID = iSelectedID;
                AreaInfo.TenantID = Convert.ToDecimal(Global.gblTenantID) ;
                dtLoad = clsArea.GetAreaMaster(AreaInfo);
                if (dtLoad.Rows.Count > 0)
                {
                    txtArea.Text = dtLoad.Rows[0]["Area"].ToString();
                    strCheck = dtLoad.Rows[0]["Area"].ToString();
                    txtRemarks.Text = dtLoad.Rows[0]["Remarks"].ToString();
                    itvwParentID = Convert.ToInt32(dtLoad.Rows[0]["ParentID"].ToString());
                    iAction = 1;
                }
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
            string strtnds = ",0,";
            TreeNode tn = trvwParentArea.SelectedNode;
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
            if (bFromEditWindowArea == false)
            {
                if (tn != null)
                {
                    itvwParentID = Convert.ToInt32(tn.Name);
                    strtnds = GetParentNodes(tn);
                }
            }
                if (iAction == 0)
                {
                    AreaInfo.AreaID = Comm.gfnGetNextSerialNo("tblArea", "AreaID");
                    if (AreaInfo.AreaID < 6)
                        AreaInfo.AreaID = 6;
                }
                else
                    AreaInfo.AreaID = iIDFromEditWindow;
                AreaInfo.Area = txtArea.Text;
                AreaInfo.Remarks = txtRemarks.Text;
                if (tn != null)
                    AreaInfo.ParentID = itvwParentID.ToString();
                else
                    AreaInfo.ParentID = "0";

                    AreaInfo.HID = strtnds;

                DataTable dtcount = new DataTable();
                dtcount = clsMaster.GetChildCount(AreaInfo.HID);
                if (dtcount.Rows.Count < 9)
                {
                    string name = Environment.MachineName;
                    AreaInfo.SystemName = name;
                    AreaInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                    AreaInfo.LastUpdateDate = DateTime.Today;
                    AreaInfo.LastUpdateTime = DateTime.Now;
                    AreaInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                    strRet = clsArea.InsertUpdateDeleteAreaMaster(AreaInfo, iAction);
                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Area name(" + txtArea.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtArea.Focus();
                                txtArea.SelectAll();
                            }
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Save", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                        {
                            if (CtrlPassed is TextBox)
                            {
                                CtrlPassed.Text = txtArea.Text;
                                CtrlPassed.Tag = AreaInfo.AreaID;
                                CtrlPassed.Name = txtArea.Name;
                                CtrlPassed.Focus();
                            }
                            this.Close();
                        }
                        else
                           ClearAll();
                        SetDefaultTreeview();
                        Comm.MessageboxToasted("Area", "Area saved successfully");

                        if (bFromEditWindowArea == true && bDirectDelete == false)
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    txtArea.Focus();
                    txtArea.SelectAll();
                    MessageBox.Show("Could not allow more than 5 sub Area ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
          }
        }
        //Description :  Delete Data from Item Area table
        private void DeleteData()
        {
            string strRet = "";
            string[] strResult;
            int iParentCount = 0;
            iAction = 2;
            AreaInfo.AreaID = iIDFromEditWindow;
            AreaInfo.Area = txtArea.Text;
            AreaInfo.Remarks = txtRemarks.Text;
            AreaInfo.ParentID = "0";
            AreaInfo.HID = "";
            string name = Environment.MachineName;
            AreaInfo.SystemName = name;
            AreaInfo.UserID = Convert.ToDecimal(Global.gblUserID);
            AreaInfo.LastUpdateDate = DateTime.Today;
            AreaInfo.LastUpdateTime = DateTime.Now;
            AreaInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            TreeNode tn = trvwParentArea.SelectedNode;
            if (tn != null)
                AreaInfo.ParentID = itvwParentID.ToString();
             else
                 AreaInfo.ParentID = "0";
            if (Convert.ToInt32(tn.Name) == iIDFromEditWindow)
            {
                AreaInfo.AreaID = iIDFromEditWindow;
            }
            else
            {
                AreaInfo.AreaID = Convert.ToInt32(tn.Name);
                DataTable dt = clsArea.GetAreaMaster(AreaInfo);
                AreaInfo.Area = Convert.ToString(dt.Rows[0]["Area"]);
                AreaInfo.Remarks = Convert.ToString(dt.Rows[0]["Remarks"]);
            }
             DataTable dtParent = clsArea.CheckParentIDExists(AreaInfo.AreaID, AreaInfo.TenantID);
             iParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);
             if (iParentCount == 0)
             {
                strRet = clsArea.InsertUpdateDeleteAreaMaster(AreaInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                           MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the Area name (" + txtArea.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                           txtArea.Focus();
                           txtArea.SelectAll();
                        }
                        else if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                            MessageBox.Show("Hey! There are entries Associated with this Area [" + txtArea.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                          else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                         MessageBox.Show("Failed to Delete", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                         ClearAll();

                }
                if (bDirectDelete == true)
                {
                    ClearAll();
                }
                if (bFromEditWindowArea == true && bDirectDelete == false)
                {
                   this.Close();
                } 
             }
             else
                  MessageBox.Show("Can't Allow to delete the parent Area  (" + strSelectNodeName + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            FillTreeview();
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtArea.Clear();
            txtRemarks.Clear();
            //trvwParentArea.Nodes.Clear();
            btnDelete.Enabled = false;
            bDirectDelete = false;
            txtArea.Focus();
        }
        #endregion
    }
}

