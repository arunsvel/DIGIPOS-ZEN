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
using DigiposZen.Controls;
using System.Runtime.InteropServices;

namespace DigiposZen
{
    public partial class frmItemCategory : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description: Creation, Edit and Deletion of Category and Subcategory of the Item
        // Developed By: Dipu Joseph
        // Completed Date & Time: 07-Sep-2021 4.30 PM
        // Last Edited By:Anjitha k k
        // Last Edited Date & Time:01-March-2022 10:36 AM
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        Task task1; // = new Task();

        public frmItemCategory(int iCategoryID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

            iIDFromEditWindow = iCategoryID;
            CtrlPassed = Controlpassed;


            bFromEditWindowCategory = bFromEdit;
            //this.BackColor = Global.gblFormBorderColor;

            if (iCategoryID != 0)
            {
                LoadData(iCategoryID);
            }
            else
            {
                btnDelete.Enabled = false;
            }

            if (CtrlPassed != null && iIDFromEditWindow == 0)
            {
                txtCategoryName.Text = CtrlPassed.Text.ToString();
            }

            txtCategoryName.Focus();
            //txtCategoryName.SelectAll();
            txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;

            //lblMand1.Location = new Point(127, 23);

            if (blnDisableMinimize == true) btnMinimize.Enabled = false;

            Cursor.Current = Cursors.Default;
        }


        #region "VARIABLES -------------------------------------------- >>"
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsCategory CategoryBL = new clsCategory();
        UspGetCategoriesinfo GetCatInfo = new UspGetCategoriesinfo();
        UspInsertCategoryInfo CategoryInfo = new UspInsertCategoryInfo();
        UspGetMasterInfo GetMaster = new UspGetMasterInfo();
        clsMaster clsMaster = new clsMaster();

        //For Drag Form
        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;

        int iIDFromEditWindow;
        int itvwParentID;
        int iAction = 0;
        Control ctrl;
        string strCheck = "";
        string strSelectNodeName = "";
        string strCategoryName="";
        bool bFromEditWindowCategory;
        bool bDirectDelete = false;
        Control CtrlPassed;
        #endregion

        #region "EVENTS ----------------------------------------------- >>"
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
        private void txtCategoryName_Click(object sender, EventArgs e)
        {
            toolCategories.SetToolTip(txtCategoryName, "Specify Unique Category name");
        }
        private void txtDiscountPerc_Click(object sender, EventArgs e)
        {
            toolCategories.SetToolTip(txtDiscountPerc, "To calculate the Percentage of discount as per setting");
        }
        private void txtRemarks_Click(object sender, EventArgs e)
        {
            toolCategories.SetToolTip(txtRemarks, "User enter small description about the Category item");
        }

        private async void frmItemCategory_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                //LoadBGImage();

                string strCategoryName = txtCategoryName.Text;
                if (iIDFromEditWindow == 0)
                {
                    ClearAll();
                    SetDefaultTreeview();
                    this.Show();
                    Application.DoEvents();
                    txtCategoryName.Focus();
                    txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
                }
                else
                {
                    FillTreeview();
                    trvwParentcategory.SelectedNode = Comm.GetNodeByText(trvwParentcategory.Nodes, strCategoryName);
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtCategoryName.Text = CtrlPassed.Text.ToString();
                }

                txtCategoryName.Select();
                txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
                Cursor.Current = Cursors.Default;

                //this.BackgroundImage = global::DigiposZen.Properties.Resources.WallpaperVioletGradient2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Parent category items not available for Loading ..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void frmItemCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtCategoryName.Text != "")
                    {
                        if (txtCategoryName.Text != strCheck)
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
                //    frmEdit.Show();
                //}
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                        if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                            txtDiscountPerc.Text = "0";

                        if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Category") == false)
                        {
                            Comm.ControlEnterLeave(txtCategoryName);
                            Application.DoEvents();

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
                    if (bFromEditWindowCategory == true && bDirectDelete == false)
                    {
                        try
                        {
                            strSelectNodeName = Convert.ToString(trvwParentcategory.SelectedNode.Text);
                            int iCategoryID = GetCategoryID();
                            if (Convert.ToDecimal(iCategoryID) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Category[" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Category [" + strSelectNodeName + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter)||(e.KeyCode == Keys.Up))
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
                else
                {
                    if (e.Shift == true && e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                   else  if (e.KeyCode == Keys.Enter)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Category keypress is not working in order" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtDiscountPerc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    trvwParentcategory.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Category") == false)
                    {
                        txtRemarks.Focus();
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
                MessageBox.Show("Category keypress is not working in order" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtDiscountPerc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Discount% not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtDiscountPerc_Leave(object sender, EventArgs e)
        {
            try 
            { 
                if (txtDiscountPerc.Text == "") txtDiscountPerc.Text = "0";
                else if (txtDiscountPerc.Text.TrimEnd().TrimStart() == ".") txtDiscountPerc.Text = ".0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Category") == true)
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
        private void txtRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtDiscountPerc.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    Comm.ControlEnterLeave(txtRemarks);//For Text Casing
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show("Failed to Save..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtCategoryName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCategoryName, true);
        }
        private void txtCategoryName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCategoryName);
        }
        private void trvwParentcategory_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentcategory, true);
        }
        private void trvwParentcategory_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(trvwParentcategory, false, false);
        }
        private void txtDiscountPerc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDiscountPerc, true);
        }
        private void txtRemarks_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRemarks, true);
        }
        private void txtRemarks_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRemarks);
        }
        private void trvwParentcategory_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (btnDelete.Enabled == false)//It works only in new forms
            {
                DialogResult dlgResult = MessageBox.Show("Do you want to enable edit mode?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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
                double doubleValue;
                if (string.IsNullOrEmpty(txtDiscountPerc.Text))
                    txtDiscountPerc.Text = "0";
                Cursor.Current = Cursors.WaitCursor;
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscountPerc.Text), "Category") == false)
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
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show("Failed to Save..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            Cursor.Current = Cursors.Default;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                strSelectNodeName = Convert.ToString(trvwParentcategory.SelectedNode.Text);
                int iCategoryID = GetCategoryID();
                if (Convert.ToDecimal(iCategoryID) > 5)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Category[" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Category [" + strSelectNodeName + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Failed to Find..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtCategoryName.Text != "")
            {
                if (txtCategoryName.Text != strCheck)
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
            bool bValidation = true;
            if (txtCategoryName.Text.Trim() == "")
            {
                bValidation = false;
                MessageBox.Show("Please enter the Category Name ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCategoryName.Focus();
                txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
            }
            return bValidation;
        }
        //Description :Set Decimal Point For Discount Percentage
        public string FormatValue(double myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            if (Convert.ToString(myValue) == "")
                myValue = 0;
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
        //Description :Get and Set Parent category in Treeview
        private void FillTreeview()
        {
            DataSet ds = new DataSet();
            TreeNode parentNode;
            ds = Comm.fnGetData("SELECT CategoryID,Category,Remarks,ParentID,HID,CatDiscPer,SystemName,UserID,LastUpdateDate,LastUpdateTime FROM tblCategories WHERE ParentID=0");
            //ds = Comm.fnGetData("SELECT CategoryID,Category,Remarks,ParentID,HID,CatDiscPer,SystemName,UserID,LastUpdateDate,LastUpdateTime FROM tblCategories WHERE CategoryID=0");
            trvwParentcategory.Nodes.Clear();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                parentNode = trvwParentcategory.Nodes.Add(dr["CategoryID"].ToString(), dr["Category"].ToString());
                PopulateTreeView(Convert.ToInt32(dr["CategoryID"].ToString()), parentNode);
            }
            trvwParentcategory.ExpandAll();
        }
        //Description : Get and Set Child category in Treeview
        private void PopulateTreeView(int parentId, TreeNode parentNode) 
        {
            DataTable dtgetData = new DataTable();
            dtgetData = Comm.fnGetData("SELECT CategoryID,Category,Remarks,ParentID,HID,CatDiscPer,SystemName,UserID,LastUpdateDate,LastUpdateTime FROM tblCategories WHERE ParentID=" + parentId + "").Tables[0];
            TreeNode childNode;
            foreach (DataRow dr in dtgetData.Rows)
            {
                if (parentNode == null)
                {
                    childNode = trvwParentcategory.Nodes.Add(dr["CategoryID"].ToString(), dr["Category"].ToString());
                }
                else
                {
                    parentNode.Tag = dr["ParentID"].ToString();
                    childNode = parentNode.Nodes.Add(dr["CategoryID"].ToString(), dr["Category"].ToString());
                }
                PopulateTreeView(Convert.ToInt32(dr["CategoryID"].ToString()), childNode);
            }
        }
        //Description : Fill Data when Double Click on TreeView Node
        public void FillNodeData()
        {
            TreeNode tn = trvwParentcategory.SelectedNode;
            iIDFromEditWindow = Convert.ToInt32(tn.Name);
            bFromEditWindowCategory = true;
            LoadData(iIDFromEditWindow);
            btnDelete.Enabled = true;
            bDirectDelete = true;
            txtCategoryName.Focus();
            txtCategoryName.Select();
            txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
        }
        //Description : Get CategoryID When Double Click on Node
        private int GetCategoryID()
        {
            CategoryInfo.CategoryID = iIDFromEditWindow;
            TreeNode tn = trvwParentcategory.SelectedNode;

            if (Convert.ToInt32(tn.Name) == iIDFromEditWindow)
                CategoryInfo.CategoryID = iIDFromEditWindow;
            else
                CategoryInfo.CategoryID = Convert.ToInt32(tn.Name);

            return Convert.ToInt32(CategoryInfo.CategoryID);
        }
        //Description : Set Treeview Value as Default
        private void SetDefaultTreeview()
        {
            FillTreeview();
            decimal dCategoryID = 1;
            strCategoryName = Comm.fnGetData("Select Category From tblCategories Where CategoryID = '" + dCategoryID + "'").Tables[0].Rows[0][0].ToString();
            trvwParentcategory.SelectedNode = Comm.GetNodeByText(trvwParentcategory.Nodes, strCategoryName);
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iCategoryID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetCatInfo.CategoryID = iCategoryID;
            GetCatInfo.TenantId = Global.gblTenantID;
            dtLoad = CategoryBL.GetCategories(GetCatInfo);
            if (dtLoad.Rows.Count > 0)
            {
                txtCategoryName.Text = dtLoad.Rows[0]["Category"].ToString();
                strCheck = dtLoad.Rows[0]["Category"].ToString();
                decimal DiscPer = Convert.ToDecimal(dtLoad.Rows[0]["CatDiscPer"].ToString());
                txtDiscountPerc.Text = FormatValue(Convert.ToDouble(DiscPer), true, "#.00");
                txtRemarks.Text = dtLoad.Rows[0]["Remarks"].ToString();
                itvwParentID = Convert.ToInt32(dtLoad.Rows[0]["ParentID"].ToString());
                iAction = 1;
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                string strRet = "";
                string[] strResult;
                string strtnds = ",0,";

                TreeNode tn = trvwParentcategory.SelectedNode;
                if (bDirectDelete == true)
                {
                    iAction = 1;
                }
                if (bFromEditWindowCategory == false )
                {
                    if (tn != null)
                    {
                    itvwParentID = Convert.ToInt32(tn.Name);
                    strtnds = GetParentNodes(tn);
                    }
                }
                if (txtDiscountPerc.Text == "")
                    txtDiscountPerc.Text = "0";
           
                if (iAction == 0)
                {
                    CategoryInfo.CategoryID = Comm.gfnGetNextSerialNo("tblCategories", "CategoryId");
                    if (CategoryInfo.CategoryID < 6)
                        CategoryInfo.CategoryID = 6;
                }
                else
                    CategoryInfo.CategoryID = iIDFromEditWindow;

                CategoryInfo.Category = txtCategoryName.Text;
                CategoryInfo.Remarks = txtRemarks.Text;

                if (tn != null)
                    CategoryInfo.ParentID = itvwParentID.ToString();
                else
                {
                    if (iAction == 1)
                        CategoryInfo.ParentID = itvwParentID.ToString();
                    else
                        CategoryInfo.ParentID = "0";
                }
                CategoryInfo.HID = strtnds;
                DataTable dtcount = new DataTable();
                dtcount = clsMaster.GetChildCount(CategoryInfo.HID);
                if (dtcount.Rows.Count < 9)
                {
                    CategoryInfo.CatDiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
                    CategoryInfo.SystemName = Global.gblSystemName;
                    CategoryInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                    CategoryInfo.TenantId = Convert.ToDecimal(Global.gblTenantID);
                    CategoryInfo.LastUpdateDate = DateTime.Today;
                    CategoryInfo.LastUpdateTime = DateTime.Now;
                    strRet = CategoryBL.InsertUpdateDeleteCategory(CategoryInfo, iAction);
                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the category (" + txtCategoryName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtCategoryName.Focus();
                                txtCategoryName.SelectAll();
                                txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
                            }
                            else
                            {
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Save...", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                        {
                            //CtrlPassed.Text = CtrlPassed.Text + "," + txtCategoryName.Text;
                            //CtrlPassed.Tag = CtrlPassed.Tag + "," + CategoryInfo.CategoryID;

                            CtrlPassed.Text = txtCategoryName.Text;
                            CtrlPassed.Tag = CategoryInfo.CategoryID;
                            CtrlPassed.Focus();
                            txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
                            this.Close();
                        }
                        else
                        {
                            if (bFromEditWindowCategory == false || bDirectDelete == true)
                            {
                                if (bDirectDelete == true)
                                {
                                    bFromEditWindowCategory = true;
                                    iAction = 0;
                                }
                                ClearAll();
                                SetDefaultTreeview();
                                Cursor.Current = Cursors.WaitCursor;
                                Comm.MessageboxToasted("Categories", "Category details saved successfully");
                                Cursor.Current = Cursors.Default;
                            }
                        }

                        if (bFromEditWindowCategory == true && bDirectDelete == false)
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    txtCategoryName.Focus();
                    txtCategoryName.SelectAll();
                    txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
                    MessageBox.Show("Could not allow more than 5 sub categories", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        //Description :  Delete Data from Item Category table
        private void DeleteData()
        {
            string strRet = "";
            string[] strResult;
            string strtnds = ",0,";
            int iParentCount = 0;

            iAction = 2;
            TreeNode tn = trvwParentcategory.SelectedNode;
            CategoryInfo.CategoryID = iIDFromEditWindow;
            CategoryInfo.Category = txtCategoryName.Text;
            CategoryInfo.Remarks = txtRemarks.Text;
            if (tn != null)
                CategoryInfo.ParentID = itvwParentID.ToString();
            else
            {
                if (iAction == 1)
                    CategoryInfo.ParentID = itvwParentID.ToString();
                else
                    CategoryInfo.ParentID = "0";
            }
            CategoryInfo.HID = strtnds;
            CategoryInfo.CatDiscPer = Convert.ToDecimal(txtDiscountPerc.Text);
            CategoryInfo.SystemName = Global.gblSystemName;
            CategoryInfo.UserID = Convert.ToDecimal(Global.gblUserID);
            CategoryInfo.TenantId = Convert.ToDecimal(Global.gblTenantID);
            CategoryInfo.LastUpdateDate = DateTime.Today;
            CategoryInfo.LastUpdateTime = DateTime.Now;
            TreeNode tnn = trvwParentcategory.SelectedNode;
            if (tn != null)
            {
                itvwParentID = Convert.ToInt32(tn.Name);
                strtnds = GetParentNodes(tn);
            }
            else
                itvwParentID = Convert.ToInt32(CategoryInfo.CategoryID);
            itvwParentID = Convert.ToInt32(CategoryInfo.CategoryID);

            if (Convert.ToInt32(tn.Name) == iIDFromEditWindow)
            {
                CategoryInfo.CategoryID = iIDFromEditWindow;

                DataTable dtParent = CategoryBL.CheckParentIDExists(itvwParentID, CategoryInfo.TenantId);
                iParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);
            }
            else
            {
                CategoryInfo.CategoryID = Convert.ToInt32(tn.Name);
                DataTable dtParent = CategoryBL.CheckParentIDExists(Convert.ToDecimal(tn.Name), CategoryInfo.TenantId);//Check parent exist or not when delete Child
                iParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);

                GetCatInfo.CategoryID = Convert.ToDecimal(tn.Name);
                GetCatInfo.TenantId = Global.gblTenantID;
                DataTable dt = CategoryBL.GetCategories(GetCatInfo);
                CategoryInfo.Category = Convert.ToString(dt.Rows[0]["Category"]);
                CategoryInfo.Remarks = Convert.ToString(dt.Rows[0]["Remarks"]);
                decimal Discper = Convert.ToDecimal(dt.Rows[0]["CatDiscPer"]);

                CategoryInfo.HID = strtnds;
                CategoryInfo.CatDiscPer = Discper;
                itvwParentID = Convert.ToInt32(CategoryInfo.CategoryID);
                CategoryInfo.CategoryID = Convert.ToDecimal(tn.Name);
            }
            if (iParentCount == 0)
            {
                GetMaster.TYPE = "CATEGORY";
                GetMaster.ID = Convert.ToInt32(CategoryInfo.CategoryID);
                DataTable dtMaster = new DataTable();
                dtMaster = clsMaster.GetColumnIDsData(GetMaster);
                if (dtMaster.Rows.Count == 0)
                {
                    strRet = CategoryBL.InsertUpdateDeleteCategory(CategoryInfo, iAction);
                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                                MessageBox.Show("Duplicate Entry, User has restricted to enter duplicate values in the category(" + txtCategoryName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                MessageBox.Show("Hey! There are Items Associated with this category [" + txtCategoryName.Text + "] .Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Delete  ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            ClearAll();
                    }
                    FillTreeview();
                    if (bDirectDelete == true)
                    {
                        ClearAll();
                    }
                    if (bFromEditWindowCategory == true && bDirectDelete == false)
                    {
                        this.Close();
                    }
                }
                else
                    MessageBox.Show("Can't allow to delete category [" + txtCategoryName.Text + "] is  using in Item master", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Can't allow to delete the parent category  (" + strSelectNodeName + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtCategoryName.Clear();
            txtRemarks.Clear();
            btnDelete.Enabled = false;
            txtDiscountPerc.Text = "0";
            bDirectDelete = false;
            txtCategoryName.Focus();
            txtCategoryName.SelectionStart = txtCategoryName.Text.ToString().Length;
        }
        #endregion

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {

        }

        private async void frmItemCategory_Activated(object sender, EventArgs e)
        {
            //Comm.LoadBGImage(this, picBackground);
        }

        private void picBackground_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BackgroundImage = picBackground.Image;
        }
    }
}

