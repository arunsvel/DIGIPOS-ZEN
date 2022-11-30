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
    public partial class frmCostCentre : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description:             Cost Centre Creation
        // Developed By:            Anjitha K K
        // Completed Date & Time:   28/02/2022 11.00 AM
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

        public frmCostCentre(int iCCID = 0,bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

                lblSave.ForeColor = Color.Black;

                btnSave.Image = global::InventorSync.Properties.Resources.save240402;
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

            iIDFromEditWindow = iCCID;
            bFromEditWindowCostCentre = bFromEdit;
            CtrlPassed = Controlpassed;
            this.BackColor = Global.gblFormBorderColor;
            if (iCCID != 0)
            {
                LoadData(iCCID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtCostCentreName.Focus();
            txtCostCentreName.SelectAll();
            Cursor.Current = Cursors.Default;
        }
        #region "VARIABLES  -------------------------------------------- >>"
        //info
        UspGetCostCentreInfo GetCostCentreinfo = new UspGetCostCentreInfo();
        UspCostCentreInsertInfo CostCentrinfo = new UspCostCentreInsertInfo();

        //Class
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsCostCentre clsCostCtr = new clsCostCentre();

        bool dragging = false, bValidate = true, bFromEditWindowCostCentre;
        int xOffset = 0, yOffset = 0, iAction=0, iIDFromEditWindow;
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
            toolTipState.SetToolTip(txtCostCentreName, "Specify the Unique Cost Centre");
        }
        private void txtDescription1_Click(object sender, EventArgs e)
        {
            toolTipState.SetToolTip(txtDescription1, "Enter Description About the Cost Centre");
        }
        private void txtCostCentreName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCostCentreName, true);
        }
        private void txtDescription1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription1, true);
        }
        private void txtCostCentreName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCostCentreName);
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
                txtCostCentreName.Focus();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                Comm.ControlEnterLeave(txtDescription1);
                btnSave_Click(sender,e);
            }
        }

        private void frmCostCentre_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    this.Show();
                    Application.DoEvents();
                    txtCostCentreName.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load Cost Centre  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void frmCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)
                {
                    if (txtCostCentreName.Text != "")
                    {
                        if (txtCostCentreName.Text != strCheck)
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
                    if (bFromEditWindowCostCentre == true)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Cost Centre[" + txtCostCentreName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Cost Centre [" + txtCostCentreName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Cost Centre[" + txtCostCentreName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                       DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Cost Centre [" + txtCostCentreName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtCostCentreName.Text != "")
            {
                if (txtCostCentreName.Text != strCheck)
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
            frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
            frmEdit.Show();
        }
        #endregion

        #region "METHODS --------------------------------------------- >>"
        //Description: Validate Cost Centre Field 
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
            if (txtCostCentreName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Cost Centre", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtCostCentreName.Focus();
            }
            return bValidate;
        }
        //Description: Load Data From Database
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetCostCentreinfo.CCID = Convert.ToDecimal(iSelectedID);
            GetCostCentreinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsCostCtr.GetCostCentre(GetCostCentreinfo);
            if (dtLoad.Rows.Count > 0)
            {
                txtCostCentreName.Text = dtLoad.Rows[0]["CCName"].ToString();
                strCheck = dtLoad.Rows[0]["CCName"].ToString();
                txtDescription1.Text = dtLoad.Rows[0]["Description1"].ToString();
                iAction = 1;
            }
        }
        //Description:Save Date to Cost centre table
        private void SaveData()
        {
            string[] strResult;
            string sRet = "";
            if (IsValidate() == true)
            {
                if (iAction == 0)
                {
                    CostCentrinfo.CCID = Comm.gfnGetNextSerialNo("tblCostCentre", "CCID");
                    if (CostCentrinfo.CCID < 6)
                        CostCentrinfo.CCID = 6;
                }
                else
                    CostCentrinfo.CCID = Convert.ToDecimal(iIDFromEditWindow);

                CostCentrinfo.CCName = txtCostCentreName.Text.TrimStart().TrimEnd();
                CostCentrinfo.InCharge = "";
                CostCentrinfo.Description1 = txtDescription1.Text.TrimStart().TrimEnd();
                CostCentrinfo.Description2="";
                CostCentrinfo.Description3 = "";
                CostCentrinfo.BLNDAMAGED = 0;
                CostCentrinfo.SystemName = Environment.MachineName;
                CostCentrinfo.UserID = Convert.ToDecimal(Global.gblUserID);
                CostCentrinfo.LastUpdateDate = DateTime.Today;
                CostCentrinfo.LastUpdateTime = DateTime.Now;
                CostCentrinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);

                sRet = clsCostCtr.InsertUpdateDeleteCostCentre(CostCentrinfo, iAction);
                if (sRet.Length > 2)
                {
                    strResult = sRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                                MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the Cost Centre(" + txtCostCentreName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtCostCentreName.Focus();
                            txtCostCentreName.SelectAll();
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ClearAll();
                        if (bFromEditWindowCostCentre == true)
                        {
                            this.Close();
                        }
                        Comm.MessageboxToasted("Cost Centre", "Cost Centre Saved Successfully");
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
                            CtrlPassed.Text = txtCostCentreName.Text;
                            CtrlPassed.Tag = CostCentrinfo.CCID;
                            CtrlPassed.Name = txtCostCentreName.Name;
                            CtrlPassed.Focus();
                        }
                        this.Close();
                    }
                    else
                    {
                        ClearAll();
                    }
                    Comm.MessageboxToasted("Cost Centre", "Cost Centre Saved Successfully");
                    if (bFromEditWindowCostCentre == true)
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
            CostCentrinfo.CCID = iIDFromEditWindow;
            CostCentrinfo.CCName = txtCostCentreName.Text.TrimStart().TrimEnd();
            CostCentrinfo.InCharge = "";
            CostCentrinfo.Description1 = txtDescription1.Text.TrimStart().TrimEnd();
            CostCentrinfo.Description2 = "";
            CostCentrinfo.Description3 = "";
            CostCentrinfo.BLNDAMAGED = 0;
            CostCentrinfo.SystemName = Environment.MachineName;
            CostCentrinfo.UserID = Convert.ToDecimal(Global.gblUserID);
            CostCentrinfo.LastUpdateDate = DateTime.Today;
            CostCentrinfo.LastUpdateTime = DateTime.Now;
            CostCentrinfo.TenantID = Convert.ToDecimal(Global.gblTenantID);

            sRet = clsCostCtr.InsertUpdateDeleteCostCentre(CostCentrinfo, iAction);
            if (sRet.Length > 2)
            {
                strResult = sRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                       if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                        MessageBox.Show("Hey! There are entries associated with this Cost Centre(" + txtCostCentreName.Text + ").Please Check", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (bFromEditWindowCostCentre == true)
            {
                this.Close();
            }
        }
        //Description:Clear All control
        private void ClearAll()
        {
            txtCostCentreName.Text = "";
            txtDescription1.Text = "";
            txtCostCentreName.Focus();
        }
        #endregion
    }
}
