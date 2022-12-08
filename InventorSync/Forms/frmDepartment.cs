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
    public partial class frmDepartment : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description:             Department Creation
        // Developed By:            Anjitha K K
        // Completed Date & Time:   18/03/2022 01:30 PM
        // Last Edited By:          
        // Last Edited Date & Time: 
        // ======================================================== >>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmDepartment(int iDepID = 0,bool bFromEdit = false, int DepartmentType = 0, Control Controlpassed = null)
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

            iIDFromEditWindow = iDepID;
            bFromEditWindowDepartment = bFromEdit;
            CtrlPassed = Controlpassed;
            this.BackColor = Global.gblFormBorderColor;
            if (iIDFromEditWindow != 0)
            {
                LoadData(iIDFromEditWindow);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtDepartmentName.Focus();
            txtDepartmentName.SelectAll();

            if (DepartmentType == 0)
                lblHeading.Text = "Stock Department";
            else
                lblHeading.Text = "Department";

            mDepartmentType = DepartmentType;

            Cursor.Current = Cursors.Default;
        }
        #region "VARIABLES  -------------------------------------------- >>"
        //info
        UspGetDepartmentInfo GetDepartmentinfo = new UspGetDepartmentInfo();
        UspDepartmentInsertInfo Departmentinfo = new UspDepartmentInsertInfo();

        //Class
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsDepartment clsDepart = new clsDepartment();

        bool dragging = false, bValidate = true, bFromEditWindowDepartment;
        int xOffset = 0, yOffset = 0, iAction=0, iIDFromEditWindow;
        int mDepartmentType = 0;
        string strCheck;
        Control ctrl;
        Control CtrlPassed;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        private void tlpHeading_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;

            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpHeading_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }
        private void lblHeading_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;

            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void lblHeading_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }
        private void lblHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void txtCostCentreName_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(txtDepartmentName, "Specify the Unique Department Name");
        }
        private void txtDescription1_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(txtDescription1, "Enter Description About the Department");
        }
        private void txtCostCentreName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDepartmentName, true);
        }
        private void txtDescription1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription1, true);
        }
        private void txtCostCentreName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDepartmentName);
        }
        private void txtDescription1_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription1);
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
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
                        this.SelectNextControl(ctrl, false, false, false, false);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void txtDescription1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtDepartmentName.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                Comm.ControlEnterLeave(txtDescription1);
                btnSave_Click(sender,e);
            }
        }

        private void frmDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)
                {
                    if (txtDepartmentName.Text != "")
                    {
                        if (txtDepartmentName.Text != strCheck)
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
                else if (e.KeyCode == Keys.F5)
                {
                    if (IsValidate() == true)
                    {
                        SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)
                {
                    if (bFromEditWindowDepartment == true)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Department[" + txtDepartmentName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Department [" + txtDepartmentName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            Cursor.Current = Cursors.Default;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Shortcut keys not working properly  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                        }
                    }
                }
                //else if (e.KeyCode == Keys.F3)
                //{
                //    frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
                //    frmEdit.Show();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Shortcut Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            Cursor.Current = Cursors.Default;
        }
        private void frmDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    this.Show();
                    Application.DoEvents();
                    txtDepartmentName.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load Department  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtDepartmentName.Text != "")
            {
                if (txtDepartmentName.Text != strCheck)
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
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                frmEditWindow frmEdit;
                if (mDepartmentType == 0)
                    frmEdit = new frmEditWindow("FRMSTOCKDEPARTMENT", this.MdiParent);
                else
                    frmEdit = new frmEditWindow("FRMDEPARTMENT", this.MdiParent);
                
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
                MessageBox.Show("Failed to Save" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Department[" + txtDepartmentName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                       DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Department [" + txtDepartmentName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed Delete" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                sqlControl rs = new sqlControl();

                //string json = JSONGenerator();

                //string[] param = { "@JSON" };
                //string[] values = { json };

                //rs.insertsp("SaveDepartment", param, values);


                rs.Open("Select invno, invdate, billamt from tblpurchase");
                if (!rs.eof())
                {
                    for (int i = 0 ; i < rs.FieldCount() ; i++ )
                    {
                        MessageBox.Show(rs.FieldName(i));
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region "JSONMANAGER"
        private string JSONGenerator()
        {
            string strJSON = "";

            Departmentinfo.DepartmentID = Convert.ToInt32(iIDFromEditWindow);

            Departmentinfo.Department = txtDepartmentName.Text.TrimStart().TrimEnd();
            Departmentinfo.Description = txtDescription1.Text.TrimStart().TrimEnd();
            Departmentinfo.SystemName = Environment.MachineName;
            Departmentinfo.UserID = Convert.ToDecimal(Global.gblUserID);
            Departmentinfo.LastUpdateDate = DateTime.Today;
            Departmentinfo.LastUpdateTime = DateTime.Now;
            Departmentinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            Departmentinfo.DepartmentType = mDepartmentType;

            strJSON = Newtonsoft.Json.JsonConvert.SerializeObject(Departmentinfo);

            return strJSON;
        }
        #endregion

        #region "METHODS --------------------------------------------- >>"
        //Description: Validate Department Field 
        private bool IsValidate()
        {
            if (txtDepartmentName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Department", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtDepartmentName.Focus();
            }
            return bValidate;
        }

        private void txtDepartmentName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (iIDFromEditWindow == 0)
                ShowItemSearchDetailsinGrid();

        }
        //Description : Show ItemName and Item Code when write 3 letter in  Itemname Textbox
        public void ShowItemSearchDetailsinGrid(bool blnClose = false)
        {
            if (blnClose == false)
            {
                if (txtDepartmentName.Text.Trim().Length >= 3)
                {
                    string a = txtDepartmentName.Text;
                    string sQuery = "Select Department,Description,DepartmentID From tblDepartment where brandName LIKE '" + txtDepartmentName.Text.Replace("'", "''").TrimStart().TrimEnd() + "%' And TenantID = '" + Global.gblTenantID + "' order by Department";
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

        //Description: Load Data From Database
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetDepartmentinfo.DepartmentID = Convert.ToInt32(iSelectedID);
            GetDepartmentinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsDepart.GetDepartment(GetDepartmentinfo);
            if (dtLoad.Rows.Count > 0)
            {
                txtDepartmentName.Text = dtLoad.Rows[0]["Department"].ToString();
                strCheck = dtLoad.Rows[0]["Department"].ToString();
                txtDescription1.Text = dtLoad.Rows[0]["Description"].ToString();
                iAction = 1;
            }
        }
        //Description:Save Date to Department table
        private void SaveData()
        {
            string[] strResult;
            string sRet = "";
            if (IsValidate() == true)
            {
                if (iAction == 0)
                {
                    Departmentinfo.DepartmentID = Comm.gfnGetNextSerialNo("tblDepartment", "DepartmentID");
                    if (Departmentinfo.DepartmentID < 6)
                        Departmentinfo.DepartmentID = 6;
                }
                else
                    Departmentinfo.DepartmentID = Convert.ToInt32(iIDFromEditWindow);

                Departmentinfo.Department = txtDepartmentName.Text.TrimStart().TrimEnd();
                Departmentinfo.Description = txtDescription1.Text.TrimStart().TrimEnd();
                Departmentinfo.SystemName = Environment.MachineName;
                Departmentinfo.UserID = Convert.ToDecimal(Global.gblUserID);
                Departmentinfo.LastUpdateDate = DateTime.Today;
                Departmentinfo.LastUpdateTime = DateTime.Now;
                Departmentinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                Departmentinfo.DepartmentType = mDepartmentType;

                sRet = clsDepart.InsertUpdateDeleteDepartment(Departmentinfo, iAction);
                if (sRet.Length > 2)
                {
                    strResult = sRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                                MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the Department(" + txtDepartmentName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtDepartmentName.Focus();
                            txtDepartmentName.SelectAll();
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ClearAll();
                        if (bFromEditWindowDepartment == true)
                        {
                            this.Close();
                        }
                        Comm.MessageboxToasted("Department", "Department Saved Successfully");
                    }
                }
                else
                {
                    if (Convert.ToInt32(sRet) == -1)
                        MessageBox.Show("Failed to Save! ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        if (CtrlPassed is TextBox)
                        {
                            CtrlPassed.Text = txtDepartmentName.Text;
                            CtrlPassed.Tag = Departmentinfo.DepartmentID;
                            CtrlPassed.Focus();
                            this.Close();
                        }
                        else if (CtrlPassed is ComboBox)
                        {
                            DataTable dtBrand = Comm.fnGetData("SELECT DepartmentID,Department FROM tblDepartment WHERE TenantID = " + Global.gblTenantID + " ORDER BY Department Asc").Tables[0];
                            ((ComboBox)CtrlPassed).DataSource = dtBrand;
                            ((ComboBox)CtrlPassed).DisplayMember = "Department";
                            ((ComboBox)CtrlPassed).ValueMember = "DepartmentID";
                            ((ComboBox)CtrlPassed).SelectedValue = Departmentinfo.DepartmentID;
                            ((ComboBox)CtrlPassed).Tag = Departmentinfo.DepartmentID;

                            CtrlPassed.Focus();
                            this.Close();
                        }
                    }
                    else
                    {
                        ClearAll();
                    }
                    Comm.MessageboxToasted("Department", "Department Saved Successfully");
                    if (bFromEditWindowDepartment == true)
                    {
                        this.Close();
                    }
                    
                }
            }
        }
        //Description:Delete data From table
        private void DeleteData()
        {
            string[] strResult;
            string sRet = "";

            iAction = 2;

            Departmentinfo.DepartmentID = Convert.ToInt32(iIDFromEditWindow);
            Departmentinfo.Department = txtDepartmentName.Text.TrimStart().TrimEnd();
            Departmentinfo.Description = txtDescription1.Text.TrimStart().TrimEnd();
            Departmentinfo.SystemName = Environment.MachineName;
            Departmentinfo.UserID = Convert.ToDecimal(Global.gblUserID);
            Departmentinfo.LastUpdateDate = DateTime.Today;
            Departmentinfo.LastUpdateTime = DateTime.Now;
            Departmentinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            Departmentinfo.DepartmentType = mDepartmentType;

            sRet = clsDepart.InsertUpdateDeleteDepartment(Departmentinfo, iAction);
            if (sRet.Length > 2)
            {
                strResult = sRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are Items Associated with this Department [" + txtDepartmentName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (Convert.ToInt32(sRet) == -1)
                    MessageBox.Show("Failed to Delete ?", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    ClearAll();
            }
            if (bFromEditWindowDepartment == true)
            {
                this.Close();
            }
        }
        //Description:Clear All control
        private void ClearAll()
        {
            txtDepartmentName.Text = "";
            txtDescription1.Text = "";
            txtDepartmentName.Focus();
        }
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
        #endregion
    }
}
