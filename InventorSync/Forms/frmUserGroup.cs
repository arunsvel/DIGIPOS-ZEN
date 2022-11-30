using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using InventorSync.InventorBL.Master;
using System.Runtime.InteropServices;

namespace InventorSync
{
    // ======================================================== >>
    // Description:User Group Creation
    // Developed By:Anjitha K K
    // Completed Date & Time: 19-Jan-2022 11:00 AM
    // Last Edited By:
    // Last Edited Date & Time:
    // ======================================================== >>
    public partial class frmUserGroup : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmUserGroup(int iuserGrpID = 0, bool bFromEdit = false)
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

                    tlpmain.BackColor = Color.FromArgb(249, 246, 238);
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

                iIDFromEditWindow = iuserGrpID;
                this.BackColor = Global.gblFormBorderColor;
                if (iuserGrpID != 0)
                {
                    LoadData(iuserGrpID);
                }
                else
                {
                    btnDelete.Enabled = false;
                    ClearAll();
                }
                txtUserName.Focus();
                txtUserName.SelectAll();
                bFromEditWindowUserGp = bFromEdit;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES -------------------------------------------- >>"
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetUserGroupMasterInfo GetuserInfo = new UspGetUserGroupMasterInfo();
        UspUserGroupMasterInsertInfo UserGroupInfo = new UspUserGroupMasterInsertInfo();
        clsUserGroup clsuser = new clsUserGroup();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iAction = 0;
        int iIDFromEditWindow;
        string strCheck;

        bool bFromEditWindowUserGp;
        string strRecData = "";
        string strAccessLevelSeparator = "#", strAccessLevel = "", strAccessLevelsettings = "", strAccessLevelTrans = "",
               strAccessLevelReport = "", strAccessLevelAcc = "", strAccessName = "";
        int iview = 0, iNew = 0, iEdit = 0, iCancel = 0, iDelete = 0, iPrint = 0, iDateEdit = 0;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        //Drag Form
        private void tlpmenu_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpmenu_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpmenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }
        private void tlpUser_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpUser_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpUser_MouseMove(object sender, MouseEventArgs e)
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

        private void rdoSettings_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(1);
        }
        private void rdoTransaction_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(2);
        }
        private void rdoReports_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(3);
        }
        private void rdoAccounts_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(4);
        }
        private void rdoOtherSettings_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(5);
        }
        private void rdoSettings_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtUserName.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    dgvsettingsMaster.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rdoTransaction_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dgvTrans.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rdoReports_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dgvReports.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rdoAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dgvaccounts.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rdoOtherSettings_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtbilldisc.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    txtUserName.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    rdoSettings.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtbilldisc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter))
                {
                    rdoOtherSettings.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    txtItemdisc.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtItemdisc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    txtbilldisc.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    txtCashdisc.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtCashdisc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    txtItemdisc.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtbilldisc_KeyPress(object sender, KeyPressEventArgs e)
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
                MessageBox.Show("Billwise Discount% not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtItemdisc_KeyPress(object sender, KeyPressEventArgs e)
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
                MessageBox.Show("Itemwise Discount% not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtCashdisc_KeyPress(object sender, KeyPressEventArgs e)
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
                MessageBox.Show("Cash Discount% not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void dgvsettingsMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    rdoSettings.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvTrans_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    rdoTransaction.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvReports_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    rdoReports.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvaccounts_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    rdoAccounts.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmUserGroup_Load(object sender, EventArgs e)
        {
            try
            {
                txtUserName.Focus();
                txtUserName.Select();
                ShowFormsAsperClick(1);

                CallGridData("MASTER");
                CallGridData("TRANSACTION");
                CallGridData("REPORTS");
                CallGridData("ACCOUNTS");

                MasterGridAlignment();
                TransactionGridAlignment();
                ReportGridAlignment();
                AccountsGridAlignment();

                gpbOtherSett.Size = new Size(827, 348);
                lblDiscHeading.Size = new Size(685, 29);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmUserGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtUserName.Text != "")
                    {
                        if (txtUserName.Text != strCheck)
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
                    try
                    {
                        if (bFromEditWindowUserGp == true)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete User Group [" + txtUserName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default User Group [" + txtUserName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                else if (e.Alt == true && e.KeyCode==Keys.S)
                {
                    rdoSettings_Click(sender,e);
                    rdoSettings.Checked = true;
                }
                else if (e.Alt == true && e.KeyCode == Keys.T)
                {
                    rdoTransaction_Click(sender, e);
                }
                else if (e.Alt == true && e.KeyCode == Keys.R)
                {
                    rdoReports_Click(sender, e);
                }
                else if (e.Alt == true && e.KeyCode == Keys.A)
                {
                    rdoAccounts_Click(sender, e);
                }
                else if (e.Alt == true && e.KeyCode == Keys.O)
                {
                    rdoOtherSettings_Click(sender, e);
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
 
        private void dgvsettingsMaster_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex == 0)
                {

                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[1];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                            chk.Value = false;
                        }
                    }

                }
                else if (e.ColumnIndex == 2 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchkNew = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[2];
                    if (Convert.ToBoolean(fchkNew.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chkNew = (DataGridViewCheckBoxCell)row.Cells[2];
                            chkNew.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chkNew = (DataGridViewCheckBoxCell)row.Cells[2];
                            chkNew.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 3 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchkEdit = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[3];
                    if (Convert.ToBoolean(fchkEdit.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chkEdit = (DataGridViewCheckBoxCell)row.Cells[3];
                            chkEdit.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chkEdit = (DataGridViewCheckBoxCell)row.Cells[3];
                            chkEdit.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 4 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchkdelete = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[4];
                    if (Convert.ToBoolean(fchkdelete.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chkdelete = (DataGridViewCheckBoxCell)row.Cells[4];
                            chkdelete.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            DataGridViewCheckBoxCell chkdelete = (DataGridViewCheckBoxCell)row.Cells[4];
                            chkdelete.Value = false;
                        }
                    }
                }
                ////Uncheck first row 
                else if (e.ColumnIndex == 1 && e.RowIndex > 0)
                {
                    int iFlag = 0;

                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[1];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 2 && e.RowIndex > 0)
                {
                    int iFlag = 0;

                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[2];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[e.RowIndex].Cells[2];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 3 && e.RowIndex > 0)
                {
                    int iFlag = 0;

                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[3];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[e.RowIndex].Cells[3];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[3];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 4 && e.RowIndex > 0)
                {
                    int iFlag = 0;

                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[0].Cells[4];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[e.RowIndex].Cells[4];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvsettingsMaster.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                                chk.Value = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvsettingsMaster_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvsettingsMaster.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void dgvsettingsMaster_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            { 
                foreach (DataGridViewRow dRow in dgvsettingsMaster.Rows)
                {
                    DataGridViewTextBoxCell cellColumnName = (DataGridViewTextBoxCell)dRow.Cells["Allow User Access To :"];

                    if (cellColumnName.Value.ToString() == "MASTERS")
                    {
                        dRow.DefaultCellStyle.ForeColor = Color.Blue;
                        dRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dRow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
                        dRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvTrans_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                int iFlag = 0;

                if (e.ColumnIndex == 1 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[1];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                            chk.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 2 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[2];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                            chk.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 3 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[3];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[3];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[3];
                            chk.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 4 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[4];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                            chk.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 5 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[5];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[5];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[5];
                            chk.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 6 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[6];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[6];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[6];
                            chk.Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 7 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[7];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[7];
                            chk.Value = true;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[7];
                            chk.Value = false;
                        }
                    }
                }

                else if (e.ColumnIndex == 1 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[1];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 2 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[2];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[2];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 3 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[3];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[3];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[3];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 4 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[4];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[4];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 5 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[5];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[5];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[5];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 6 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[6];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[6];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[6];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 7 && e.RowIndex > 0)
                {
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvTrans.Rows[0].Cells[7];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvTrans.Rows[e.RowIndex].Cells[7];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvTrans.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[7];
                                chk.Value = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvTrans_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvTrans.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void dgvTrans_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow dRow in dgvTrans.Rows)
                {
                    DataGridViewTextBoxCell cellColumnName = (DataGridViewTextBoxCell)dRow.Cells["Allow User Access To :"];

                    if (cellColumnName.Value.ToString() == "TRANSACTION")
                    {
                        dRow.DefaultCellStyle.ForeColor = Color.Blue;
                        dRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dRow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
                        dRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvReports_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchk = (DataGridViewCheckBoxCell)dgvReports.Rows[0].Cells[1];
                    if (Convert.ToBoolean(fchk.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvReports.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                            chk.Value = true;
                            // chk.Value = !(chk.Value == null ? false : (bool)chk.Value);
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvReports.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                            chk.Value = false;
                            // chk.Value = !(chk.Value == null ? false : (bool)chk.Value);
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex > 0)
                {
                    int iFlag = 0;

                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvReports.Rows[0].Cells[1];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvReports.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvReports.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                                chk.Value = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvReports.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void dgvReports_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
            {
                try
                { 
                foreach (DataGridViewRow dRow in dgvReports.Rows)
                {
                    DataGridViewTextBoxCell cellColumnName = (DataGridViewTextBoxCell)dRow.Cells["Allow User Access To :"];

                    if (cellColumnName.Value.ToString() == "REPORTS")
                    {
                        dRow.DefaultCellStyle.ForeColor = Color.Blue;
                        dRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dRow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
                        dRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvaccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iFlag = 0;

                if (e.ColumnIndex == 1 && e.RowIndex == 0)
                {
                    DataGridViewCheckBoxCell fchkAcc = (DataGridViewCheckBoxCell)dgvaccounts.Rows[0].Cells[1];
                    if (Convert.ToBoolean(fchkAcc.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 14)
                            {
                                DataGridViewCheckBoxCell chkAcc = (DataGridViewCheckBoxCell)row.Cells[1];
                                chkAcc.Value = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag <= 14)
                            {
                                DataGridViewCheckBoxCell chkAcc = (DataGridViewCheckBoxCell)row.Cells[1];
                                chkAcc.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 14)
                {
                    DataGridViewCheckBoxCell fchkAnal = (DataGridViewCheckBoxCell)dgvaccounts.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(fchkAnal.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag > 14 && iFlag <= 25)
                            {
                                DataGridViewCheckBoxCell chkAnal = (DataGridViewCheckBoxCell)row.Cells[1];
                                chkAnal.Value = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag > 14 && iFlag <= 25)
                            {
                                DataGridViewCheckBoxCell chkAnal = (DataGridViewCheckBoxCell)row.Cells[1];
                                chkAnal.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 25)
                {
                    DataGridViewCheckBoxCell fchkcmd = (DataGridViewCheckBoxCell)dgvaccounts.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(fchkcmd.Value) == false)
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag > 25)
                            {
                                DataGridViewCheckBoxCell chkcmd = (DataGridViewCheckBoxCell)row.Cells[1];
                                chkcmd.Value = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlag = iFlag + 1;
                            if (iFlag > 25)
                            {
                                DataGridViewCheckBoxCell chkcmd = (DataGridViewCheckBoxCell)row.Cells[1];
                                chkcmd.Value = false;
                            }
                        }
                    }
                }
                ////Uncheck First Row 
                else if (e.ColumnIndex == 1 && (e.RowIndex > 0 && e.RowIndex < 14))
                {
                    int iFlagAcc = 0;
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvaccounts.Rows[0].Cells[1];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvaccounts.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlagAcc = iFlagAcc + 1;
                            if (iFlagAcc <= 1)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && (e.RowIndex > 14 && e.RowIndex < 25))
                {
                    int iFlagAcc = 0;
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvaccounts.Rows[14].Cells[1];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvaccounts.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlagAcc = iFlagAcc + 1;
                            if (iFlagAcc == 15)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                                chk.Value = false;
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex > 25)
                {
                    int iFlagAcc = 0;
                    DataGridViewCheckBoxCell chkall = (DataGridViewCheckBoxCell)dgvaccounts.Rows[25].Cells[1];
                    DataGridViewCheckBoxCell chkmid = (DataGridViewCheckBoxCell)dgvaccounts.Rows[e.RowIndex].Cells[1];
                    if (Convert.ToBoolean(chkmid.Value) == true && Convert.ToBoolean(chkall.Value) == true)
                    {
                        foreach (DataGridViewRow row in dgvaccounts.Rows)
                        {
                            iFlagAcc = iFlagAcc + 1;
                            if (iFlagAcc == 26)
                            {
                                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[1];
                                chk.Value = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dgvaccounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvaccounts.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void dgvaccounts_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow dRow in dgvaccounts.Rows)
                {
                    DataGridViewTextBoxCell cellColumnName = (DataGridViewTextBoxCell)dRow.Cells["Allow User Access To :"];

                    if (cellColumnName.Value.ToString() == "ACCOUNTS")
                    {
                        dRow.DefaultCellStyle.ForeColor = Color.Blue;
                        dRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dRow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
                        dRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                    if (cellColumnName.Value.ToString() == "ANALYSIS")
                    {
                        dRow.DefaultCellStyle.ForeColor = Color.Blue;
                        dRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dRow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
                        dRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                    if (cellColumnName.Value.ToString() == "COMMAND WINDOW")
                    {
                        dRow.DefaultCellStyle.ForeColor = Color.Blue;
                        dRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dRow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
                        dRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtUserName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUserName, true);
        }
        private void txtUserName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUserName);
        }
        private void txtbilldisc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtbilldisc, true);
        }
        private void txtbilldisc_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtbilldisc.Text))
                    txtbilldisc.Text = "0";
                Comm.ControlEnterLeave(txtbilldisc, false, false);
                txtbilldisc.Text = FormatValue(Convert.ToDouble(txtbilldisc.Text), true, "#.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtItemdisc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtItemdisc, true);
        }
        private void txtItemdisc_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtItemdisc.Text))
                    txtItemdisc.Text = "0";
                Comm.ControlEnterLeave(txtItemdisc, false, false);
                txtItemdisc.Text = FormatValue(Convert.ToDouble(txtItemdisc.Text), true, "#.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtCashdisc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCashdisc, true);
        }
        private void txtCashdisc_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtCashdisc.Text))
                    txtCashdisc.Text = "0";
                Comm.ControlEnterLeave(txtCashdisc, false, false);
                txtCashdisc.Text = FormatValue(Convert.ToDouble(txtCashdisc.Text), true, "#.00");
            }
            catch (Exception ex)
            {
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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete User Group[" + txtUserName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default User Group [" + txtUserName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                if (txtUserName.Text != "")
                {
                    if (txtUserName.Text != strCheck)
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
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            try
            {
                bool bValidate = true;
                if (txtUserName.Text.Trim() == "")
                {
                    bValidate = false;
                    MessageBox.Show("Please enter the User group name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtUserName.Focus();
                }
                return bValidate;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        //Description :Set Decimal Point For Discount Percentage
        public string FormatValue(double myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "0";
            }
        }
        //Description : Show Form Asper when Click on Button
        private void ShowFormsAsperClick(int iColIndex = 1)
        {
            try
            {
                for (int g = 0; g < this.tlpgroupbox.ColumnCount; g++)
                {
                    if (iColIndex == g + 1)
                    {
                        this.tlpgroupbox.ColumnStyles[g].SizeType = SizeType.Percent;
                        this.tlpgroupbox.ColumnStyles[g].Width = 100;
                    }
                    else
                    {
                        this.tlpgroupbox.ColumnStyles[g].SizeType = SizeType.Absolute;
                        this.tlpgroupbox.ColumnStyles[g].Width = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description :Grid Master Alignment
        private void MasterGridAlignment()
        {
            try
            {
                dgvsettingsMaster.Columns[0].ReadOnly = true;
                dgvsettingsMaster.Columns[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F);
                dgvsettingsMaster.Columns[0].Width = 380;
                dgvsettingsMaster.Columns[1].Width = 80;
                dgvsettingsMaster.Columns[2].Width = 80;
                dgvsettingsMaster.Columns[3].Width = 80;
                dgvsettingsMaster.Columns[4].Width = 80;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description :Grid Transaction Alignment
        private void TransactionGridAlignment()
        {
            try
            {
                dgvTrans.Columns[0].ReadOnly = true;
                dgvTrans.Columns[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F);
                dgvTrans.Columns[0].Width = 280;
                dgvTrans.Columns[1].Width = 60;
                dgvTrans.Columns[2].Width = 60;
                dgvTrans.Columns[3].Width = 60;
                dgvTrans.Columns[4].Width = 60;
                dgvTrans.Columns[5].Width = 60;
                dgvTrans.Columns[6].Width = 40;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdoTransaction_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoSettings_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoReports_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoAccounts_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoOtherSettings_CheckedChanged(object sender, EventArgs e)
        {

        }

        //Description :Grid ReportAlignment
        private void ReportGridAlignment()
        {
            try
            {
                dgvReports.Columns[0].ReadOnly = true;
                dgvReports.Columns[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F);
                dgvReports.Columns[0].Width = 600;
                dgvReports.Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description :Grid Accounts Alignment
        private void AccountsGridAlignment()
        {
            try
            {
                dgvaccounts.Columns[0].ReadOnly = true;
                dgvaccounts.Columns[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F);
                dgvaccounts.Columns[0].Width = 600;
                dgvaccounts.Columns[1].Width = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description :Load Master,Report,Accounts Gid Data on Datatable
        private DataTable GridInitialize(string strItems = "", string strRecData = "", string strGridType = "")
        {
            try
            {
                string[] strArrItems = strItems.Split(',');
                string[] strArrFillItems;
                DataTable dtRet = new DataTable();
                dtRet.Columns.Add("Allow User Access To :", typeof(string));
                dtRet.Columns.Add("View", typeof(bool));
                if (strGridType == "MASTER")
                {
                    dtRet.Columns.Add("New", typeof(bool));
                    dtRet.Columns.Add("Edit", typeof(bool));
                    dtRet.Columns.Add("Delete", typeof(bool));
                }
                if (strArrItems.Length > 0)
                {
                    for (int i = 0; i < strArrItems.Length; i++)
                    {
                        DataRow row = dtRet.NewRow();
                        String[] separator = { "#", "<", ">" };
                        strArrFillItems = strRecData.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        row["Allow User Access To :"] = strArrItems[i];
                        row["View"] = 0;
                        if (strGridType == "MASTER")
                        {
                            row["New"] = 0;
                            row["Edit"] = 0;
                            row["Delete"] = 0;
                        }
                        if (strArrFillItems.Length > 0)
                        {
                            for (int j = 0; j < strArrFillItems.Length; j++)
                            {
                                string sRecData1 = strArrFillItems[j];
                                String[] separator1 = { "|" };
                                string[] sArrFillItems1 = sRecData1.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                                string LoadData = strArrItems[i];
                                string EditData = sArrFillItems1[0];
                                if (LoadData == EditData)
                                {
                                    row["View"] = Convert.ToInt32(sArrFillItems1[1]);
                                    if (strGridType == "MASTER")
                                    {
                                        row["New"] = Convert.ToInt32(sArrFillItems1[2]);
                                        row["Edit"] = Convert.ToInt32(sArrFillItems1[3]);
                                        row["Delete"] = Convert.ToInt32(sArrFillItems1[4]);
                                    }
                                }
                            }
                        }
                        dtRet.Rows.Add(row);
                    }
                }
                return dtRet;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return new DataTable();
            }
        }
        //Description :Load Transaction Gid Data on Datatable
        private DataTable LoadTransaction()
        {
            try
            {
                DataTable dtTransGrid = new DataTable();
                dtTransGrid.Columns.Add("Allow User Access To :", typeof(string));
                dtTransGrid.Columns.Add("View", typeof(bool));
                dtTransGrid.Columns.Add("New", typeof(bool));
                dtTransGrid.Columns.Add("Edit", typeof(bool));
                dtTransGrid.Columns.Add("Cancel", typeof(bool));
                dtTransGrid.Columns.Add("Delete", typeof(bool));
                dtTransGrid.Columns.Add("Print", typeof(bool));
                dtTransGrid.Columns.Add("Date Editable", typeof(bool));
                DataTable dtTransParent = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID From tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID= VchTypeID) ORDER BY SortOrder Asc").Tables[0];
                if (dtTransParent.Rows.Count > 0)
                {
                    DataRow row = dtTransGrid.NewRow();
                    row["Allow User Access To :"] = "TRANSACTION";
                    row["View"] = 0;
                    row["New"] = 0;
                    row["Edit"] = 0;
                    row["Cancel"] = 0;
                    row["Delete"] = 0;
                    row["Print"] = 0;
                    row["Date Editable"] = 0;
                    dtTransGrid.Rows.Add(row);
                    for (int i = 0; i < dtTransParent.Rows.Count; i++)
                    {
                        row = dtTransGrid.NewRow();
                        row["Allow User Access To :"] = dtTransParent.Rows[i][1];
                        row["View"] = 0;
                        row["New"] = 0;
                        row["Edit"] = 0;
                        row["Cancel"] = 0;
                        row["Delete"] = 0;
                        row["Print"] = 0;
                        row["Date Editable"] = 0;
                        dtTransGrid.Rows.Add(row);
                        int parentid = Convert.ToInt32(dtTransParent.Rows[i][0]);
                        DataTable dtTranschild = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID From tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID <> VchTypeID) AND (ParentID = " + parentid + ")").Tables[0];
                        if (dtTranschild.Rows.Count > 0)
                        {
                            for (int j = 0; j < dtTranschild.Rows.Count; j++)
                            {
                                row = dtTransGrid.NewRow();
                                row["Allow User Access To :"] = "          >> " + dtTranschild.Rows[j][1];
                                row["View"] = 0;
                                row["New"] = 0;
                                row["Edit"] = 0;
                                row["Cancel"] = 0;
                                row["Delete"] = 0;
                                row["Print"] = 0;
                                row["Date Editable"] = 0;
                                dtTransGrid.Rows.Add(row);
                            }
                        }
                    }
                }
                return dtTransGrid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return new DataTable();
            }
        }
        //Description :Load Transaction Gid Data on Datatable when Edit
        private DataTable LoadTransactionFill(string strRecData = "")
        {
            try
            {
                string[] strArrFillItems;
                string[] strseparatorsplit = { "#" };
                strArrFillItems = strRecData.Split(strseparatorsplit, StringSplitOptions.RemoveEmptyEntries);
                strRecData = strArrFillItems[1];
                string[] separator = { "<", ">", "          " };
                strArrFillItems = strRecData.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                DataTable dtTransGrid = new DataTable();
                dtTransGrid.Columns.Add("Allow User Access To :", typeof(string));
                dtTransGrid.Columns.Add("View", typeof(bool));
                dtTransGrid.Columns.Add("New", typeof(bool));
                dtTransGrid.Columns.Add("Edit", typeof(bool));
                dtTransGrid.Columns.Add("Cancel", typeof(bool));
                dtTransGrid.Columns.Add("Delete", typeof(bool));
                dtTransGrid.Columns.Add("Print", typeof(bool));
                dtTransGrid.Columns.Add("Date Editable", typeof(bool));
                for (int m = 0; m < strArrFillItems.Length; m++)
                {
                    string strRecData2 = strArrFillItems[m];
                    string[] strseparator2 = { "|" };
                    string[] strArrFillItems2 = strRecData2.Split(strseparator2, StringSplitOptions.RemoveEmptyEntries);
                    string EditData1 = strArrFillItems2[0];
                    DataRow row = dtTransGrid.NewRow();
                    if (EditData1 == "TRANSACTION")
                    {
                        row["Allow User Access To :"] = "TRANSACTION";
                        row["View"] = Convert.ToInt32(strArrFillItems2[1]);
                        row["New"] = Convert.ToInt32(strArrFillItems2[2]);
                        row["Edit"] = Convert.ToInt32(strArrFillItems2[3]);
                        row["Cancel"] = Convert.ToInt32(strArrFillItems2[4]);
                        row["Delete"] = Convert.ToInt32(strArrFillItems2[5]);
                        row["Print"] = Convert.ToInt32(strArrFillItems2[6]);
                        row["Date Editable"] = Convert.ToInt32(strArrFillItems2[7]);
                        dtTransGrid.Rows.Add(row);
                    }
                }
                DataTable dtTransParent = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID From tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID= VchTypeID) ORDER BY SortOrder Asc").Tables[0];
                if (dtTransParent.Rows.Count > 0)
                {
                    for (int i = 0; i < dtTransParent.Rows.Count; i++)
                    {
                        DataRow row = dtTransGrid.NewRow();
                        row["Allow User Access To :"] = dtTransParent.Rows[i][1];
                        row["View"] = 0;
                        row["New"] = 0;
                        row["Edit"] = 0;
                        row["Cancel"] = 0;
                        row["Delete"] = 0;
                        row["Print"] = 0;
                        row["Date Editable"] = 0;
                        if (strArrFillItems.Length > 0)
                        {
                            for (int k = 0; k < strArrFillItems.Length; k++)
                            {
                                string sRecData1 = strArrFillItems[k];
                                String[] separator1 = { "|" };
                                string[] sArrFillItems1 = sRecData1.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                                string strEditData = sArrFillItems1[0];
                                string strLoadData = dtTransParent.Rows[i][1].ToString();
                                if (strLoadData == strEditData)
                                {
                                    row["View"] = Convert.ToInt32(sArrFillItems1[1]);
                                    row["New"] = Convert.ToInt32(sArrFillItems1[2]);
                                    row["Edit"] = Convert.ToInt32(sArrFillItems1[3]);
                                    row["Cancel"] = Convert.ToInt32(sArrFillItems1[4]);
                                    row["Delete"] = Convert.ToInt32(sArrFillItems1[5]);
                                    row["Print"] = Convert.ToInt32(sArrFillItems1[6]);
                                    row["Date Editable"] = Convert.ToInt32(sArrFillItems1[7]);
                                    dtTransGrid.Rows.Add(row);
                                    int parentid = Convert.ToInt32(dtTransParent.Rows[i][0]);
                                    DataTable dtTranschild = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID From tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID <> VchTypeID) AND (ParentID = " + parentid + ")").Tables[0];
                                    if (dtTranschild.Rows.Count > 0)
                                    {
                                        for (int j = 0; j < dtTranschild.Rows.Count; j++)
                                        {
                                            for (int l = 0; l < strArrFillItems.Length; l++)
                                            {
                                                sRecData1 = strArrFillItems[l];
                                                string[] strseparator2 = { "|" };
                                                string[] strArrFillItems2 = sRecData1.Split(strseparator2, StringSplitOptions.RemoveEmptyEntries);
                                                strEditData = strArrFillItems2[0];
                                                strEditData = strEditData.TrimStart();
                                                string strLoadDatachild = dtTranschild.Rows[j][1].ToString();
                                                if (strLoadDatachild == strEditData)
                                                {
                                                    row = dtTransGrid.NewRow();
                                                    row["Allow User Access To :"] = "          >> " + dtTranschild.Rows[j][1];
                                                    row["View"] = Convert.ToInt32(strArrFillItems2[1]);
                                                    row["New"] = Convert.ToInt32(strArrFillItems2[2]);
                                                    row["Edit"] = Convert.ToInt32(strArrFillItems2[3]);
                                                    row["Cancel"] = Convert.ToInt32(strArrFillItems2[4]);
                                                    row["Delete"] = Convert.ToInt32(strArrFillItems2[5]);
                                                    row["Print"] = Convert.ToInt32(strArrFillItems2[6]);
                                                    row["Date Editable"] = Convert.ToInt32(strArrFillItems2[7]);
                                                    dtTransGrid.Rows.Add(row);
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return dtTransGrid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return new DataTable();
            }
        }
        //Description : Load Data on Grid
        private void CallGridData(string sGridType = "")
        {
            try
            {
                if (sGridType == "MASTER")
                {
                    string sItems = "MASTERS,ItemMaster,Category,Department,HSNCode,Manufacturer,ProductGroup,Agent,Employee,EmployeeCategory,Voucher Type,Ledger,AccountGroup,Customer,Supplier,Currency,TaxClass,Unit,State,TaxMode,GroupItem,CostCentre,CurrencyConversion,Area,SmsScheme,USER,USERGROUP";
                    if (bFromEditWindowUserGp == true)
                    {
                        strRecData = strAccessLevel;
                    }
                    dgvsettingsMaster.DataSource = GridInitialize(sItems, strRecData, sGridType);
                }
                else if (sGridType == "TRANSACTION")
                {
                    if (bFromEditWindowUserGp == true && !string.IsNullOrEmpty(strRecData))
                    {
                        strRecData = strAccessLevel;
                        dgvTrans.DataSource = LoadTransactionFill(strRecData);
                    }
                    else
                        dgvTrans.DataSource = LoadTransaction();
                }
                else if (sGridType == "REPORTS")
                {
                    string sItems = "REPORTS,StockReports,SalesReports,PurchaseReports,SalesReturnReports,PurchaseReturnReports,SalesOrderReports,PurchaseOrderReports,DeliveryNoteReports,ReceiptNoteReports,StockTransferReports,AccountsReports,AnalysisReports,PhysicalStockReports,RepackingReports,GSTReport";
                    if (bFromEditWindowUserGp == true)
                    {
                        strRecData = strAccessLevel;
                    }
                    dgvReports.DataSource = GridInitialize(sItems, strRecData, sGridType);
                }
                else if (sGridType == "ACCOUNTS")
                {
                    string sItems = "ACCOUNTS,Daybook,Cashbook,TrialBalance,ProfitandLossAccount,BalanceSheet,ReceiptPaymentsAccount,ReceiptPaymentBankColumnar,IncomeandExpenditureAccount,CashFlow,FundFlow,RatioAnalysis,NegativeLedger,BankReconciliation,ANALYSIS,Biz Search,Edit Window,Cheque Register,Item Analysis,Item View,Customer View,Supplier View,Customer Analysis,Dash Board,Tools,COMMAND WINDOW,DataMigration,BarcodeManager,BatchDecativator,Transparent,Bachup Database,Restore Database,Unicode Language Editor,BatchDeactivator,GroupSMS,Transaction Transformation,EWayBill";

                    if (bFromEditWindowUserGp == true)
                    {
                        strRecData = strAccessLevel;
                    }
                    dgvaccounts.DataSource = GridInitialize(sItems, strRecData, sGridType);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Clear Checked Data on Grid
        private void CallGridDataclear(string sGridType = "")
        {
            try
            {
                if (sGridType == "MASTER")
                {
                    string sItems = "MASTERS,ItemMaster,Category,Department,HSNCode,Manufacturer,ProductGroup,Agent,Employee,EmployeeCategory,Voucher Type,Ledger,AccountGroup,Customer,Supplier,Currency,TaxClass,Unit,State,TaxMode,GroupItem,CostCentre,CurrencyConversion,Area,SmsScheme,USER,USERGROUP";
                    dgvsettingsMaster.DataSource = GridInitialize(sItems, strRecData, sGridType);
                }
                else if (sGridType == "TRANSACTION")
                {
                    dgvTrans.DataSource = LoadTransaction();
                }
                else if (sGridType == "REPORTS")
                {
                    string sItems = "StockReports,SalesReports,PurchaseReports,SalesReturnReports,PurchaseReturnReports,SalesOrderReports,PurchaseOrderReports,DeliveryNoteReports,ReceiptNoteReports,StockTransferReports,AccountsReports,AnalysisReports,PhysicalStockReports,RepackingReports,GSTReport";
                    dgvReports.DataSource = GridInitialize(sItems, strRecData, sGridType);
                }
                else if (sGridType == "ACCOUNTS")
                {
                    string sItems = "ACCOUNTS,Daybook,Cashbook,TrialBalance,ProfitandLossAccount,BalanceSheet,ReceiptPaymentsAccount,ReceiptPaymentBankColumnar,IncomeandExpenditureAccount,CashFlow,FundFlow,RatioAnalysis,NegativeLedger,BankReconciliation,ANALYSIS,Biz Search,Edit Window,Cheque Register,Item Analysis,Item View,Customer View,Supplier View,Customer Analysis,Dash Board,Tools,COMMAND WINDOW,DataMigration,BarcodeManager,BatchDecativator,Transparent,Bachup Database,Restore Database,Unicode Language Editor,BatchDeactivator,GroupSMS,Transaction Transformation,EWayBill";
                    dgvaccounts.DataSource = GridInitialize(sItems, strRecData, sGridType);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Fill AccessLevel for Save on tblUserGroupMaster
        private string FIllAccesslevel()
        {
            try
            {
                //Settings and Master
                for (int k = 0; k < dgvsettingsMaster.Rows.Count; k++)
                {
                    DataGridViewTextBoxCell cellAccessName = (DataGridViewTextBoxCell)dgvsettingsMaster.Rows[k].Cells[0];
                    DataGridViewCheckBoxCell cellView = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[k].Cells[1];
                    DataGridViewCheckBoxCell cellNew = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[k].Cells[2];
                    DataGridViewCheckBoxCell cellEdit = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[k].Cells[3];
                    DataGridViewCheckBoxCell cellDelete = (DataGridViewCheckBoxCell)dgvsettingsMaster.Rows[k].Cells[4];
                    strAccessName = cellAccessName.Value.ToString();
                    iview = Convert.ToInt32(cellView.Value);
                    iNew = Convert.ToInt32(cellNew.Value);
                    iEdit = Convert.ToInt32(cellEdit.Value);
                    iDelete = Convert.ToInt32(cellDelete.Value);
                    strAccessLevelsettings = strAccessLevelsettings + "<" + strAccessName + "|" + iview + "|" + iNew + "|" + iEdit + "|" + iDelete + "|>";
                }
                strAccessLevelsettings = strAccessLevelSeparator + strAccessLevelsettings + strAccessLevelSeparator;
                //Transaction
                for (int k = 0; k < dgvTrans.Rows.Count; k++)
                {
                    DataGridViewTextBoxCell cellAccessName = (DataGridViewTextBoxCell)dgvTrans.Rows[k].Cells[0];
                    DataGridViewCheckBoxCell cellView = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[1];
                    DataGridViewCheckBoxCell cellNew = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[2];
                    DataGridViewCheckBoxCell cellEdit = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[3];
                    DataGridViewCheckBoxCell cellCan = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[4];
                    DataGridViewCheckBoxCell cellDelete = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[5];
                    DataGridViewCheckBoxCell cellPrint = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[6];
                    DataGridViewCheckBoxCell cellDate = (DataGridViewCheckBoxCell)dgvTrans.Rows[k].Cells[7];
                    strAccessName = cellAccessName.Value.ToString();
                    iview = Convert.ToInt32(cellView.Value);
                    iNew = Convert.ToInt32(cellNew.Value);
                    iEdit = Convert.ToInt32(cellEdit.Value);
                    iCancel = Convert.ToInt32(cellCan.Value);
                    iDelete = Convert.ToInt32(cellDelete.Value);
                    iPrint = Convert.ToInt32(cellPrint.Value);
                    iDateEdit = Convert.ToInt32(cellDate.Value);
                    strAccessLevelTrans = strAccessLevelTrans + "<" + strAccessName + "|" + iview + "|" + iNew + "|" + iEdit + "|" + iCancel + "|" + iDelete + "|" + iPrint + "|" + iDateEdit + "|>";
                }
                strAccessLevelTrans = strAccessLevelSeparator + strAccessLevelTrans + strAccessLevelSeparator;
                //Report
                for (int k = 0; k < dgvReports.Rows.Count; k++)
                {

                    DataGridViewTextBoxCell cellAccessName = (DataGridViewTextBoxCell)dgvReports.Rows[k].Cells[0];
                    DataGridViewCheckBoxCell cellView = (DataGridViewCheckBoxCell)dgvReports.Rows[k].Cells[1];
                    strAccessName = cellAccessName.Value.ToString();
                    iview = Convert.ToInt32(cellView.Value);
                    strAccessLevelReport = strAccessLevelReport + "<" + strAccessName + "|" + iview + "|>";
                }
                strAccessLevelReport = strAccessLevelSeparator + strAccessLevelReport + strAccessLevelSeparator;
                //Accounts & Analysis
                for (int k = 0; k < dgvaccounts.Rows.Count; k++)
                {
                    DataGridViewTextBoxCell cellAccessName = (DataGridViewTextBoxCell)dgvaccounts.Rows[k].Cells[0];
                    DataGridViewCheckBoxCell cellView = (DataGridViewCheckBoxCell)dgvaccounts.Rows[k].Cells[1];
                    strAccessName = cellAccessName.Value.ToString();
                    iview = Convert.ToInt32(cellView.Value);
                    strAccessLevelAcc = strAccessLevelAcc + "<" + strAccessName + "|" + iview + "|>";
                }
                strAccessLevelAcc = strAccessLevelSeparator + strAccessLevelAcc + strAccessLevelSeparator;
                strAccessLevel = strAccessLevelsettings + strAccessLevelTrans + strAccessLevelReport + strAccessLevelAcc;
                return strAccessLevel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataTable dtLoad = new DataTable();
                GetuserInfo.GroupID = Convert.ToDecimal(iSelectedID);
                GetuserInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                dtLoad = clsuser.GetUserGroupMaster(GetuserInfo);
                if (dtLoad.Rows.Count > 0)
                {
                    txtUserName.Text = dtLoad.Rows[0]["GroupName"].ToString();
                    strCheck = dtLoad.Rows[0]["GroupName"].ToString();
                    txtbilldisc.Text = dtLoad.Rows[0]["BillDisc"].ToString();
                    txtItemdisc.Text = dtLoad.Rows[0]["ItemDisc"].ToString();
                    txtCashdisc.Text = dtLoad.Rows[0]["CashDisc"].ToString();
                    strAccessLevel = dtLoad.Rows[0]["AccessLevel"].ToString();
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
            try
            {
                string[] strResult;
                string strRet = "";
                if (IsValidate() == true)
                {
                    if (iAction == 0)
                    {
                        UserGroupInfo.ID = Comm.gfnGetNextSerialNo("tblUserGroupMaster", "ID");
                        if (UserGroupInfo.ID < 6)
                            UserGroupInfo.ID = 6;
                    }
                    else
                        UserGroupInfo.ID = Convert.ToDecimal(iIDFromEditWindow);
                    strAccessLevel = FIllAccesslevel();
                    UserGroupInfo.GroupName = txtUserName.Text;
                    UserGroupInfo.AccessLevel = strAccessLevel;
                    UserGroupInfo.StrCCID = "";
                    UserGroupInfo.RptAccesslevel = "";
                    UserGroupInfo.SystemName = Global.gblSystemName;
                    UserGroupInfo.UserID = Convert.ToDecimal(Global.gblUserID);
                    UserGroupInfo.LastUpdateDate = DateTime.Today;
                    UserGroupInfo.LastUpdateTime = DateTime.Now;
                    UserGroupInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                    if (string.IsNullOrEmpty(txtbilldisc.Text))
                        txtbilldisc.Text = "0";
                    if (string.IsNullOrEmpty(txtItemdisc.Text))
                        txtItemdisc.Text = "0";
                    if (string.IsNullOrEmpty(txtCashdisc.Text))
                        txtCashdisc.Text = "0";
                    UserGroupInfo.BillDisc = float.Parse(txtbilldisc.Text);
                    UserGroupInfo.ItemDisc = float.Parse(txtItemdisc.Text);
                    UserGroupInfo.CashDisc = float.Parse(txtCashdisc.Text);
                    strRet = clsuser.InsertUpdateDeleteUserGroup(UserGroupInfo, iAction);
                    if (strRet.Length > 2)
                    {
                        strResult = strRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                            {
                                MessageBox.Show("Duplicate Entry,User has restricted to enter duplicate values in the User Group(" + txtUserName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtUserName.Focus();
                                txtUserName.SelectAll();
                            }
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (bFromEditWindowUserGp == true)
                            {
                                this.Close();
                            }
                            ClearAll();
                            Comm.MessageboxToasted("User Group", "User Group saved successfully");
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(strRet) == -1)
                            MessageBox.Show("Failed to Save! ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {

                            if (bFromEditWindowUserGp == true)
                            {
                                this.Close();
                            }
                            ClearAll();
                            Comm.MessageboxToasted("User Group", "User Group saved successfully");
                        }
                    }
                    CallGridDataclear("MASTER");
                    CallGridDataclear("TRANSACTION");
                    CallGridDataclear("REPORTS");
                    CallGridDataclear("ACCOUNTS");
                    rdoSettings.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description :  Delete Data from UserGroup table
        private void DeleteData()
        {
            try
            {
                string[] strResult;
                string strRet = "";
                iAction = 2;
                DataTable dtTaxMode = new DataTable();
                UserGroupInfo.ID = Convert.ToDecimal(iIDFromEditWindow);
                UserGroupInfo.GroupName = txtUserName.Text;
                UserGroupInfo.AccessLevel = "";
                UserGroupInfo.StrCCID = "";
                UserGroupInfo.RptAccesslevel = "";
                if (string.IsNullOrEmpty(txtbilldisc.Text))
                    txtbilldisc.Text = "0";
                if (string.IsNullOrEmpty(txtItemdisc.Text))
                    txtItemdisc.Text = "0";
                if (string.IsNullOrEmpty(txtCashdisc.Text))
                    txtCashdisc.Text = "0";
                UserGroupInfo.BillDisc = float.Parse(txtbilldisc.Text);
                UserGroupInfo.ItemDisc = float.Parse(txtItemdisc.Text);
                UserGroupInfo.CashDisc = float.Parse(txtCashdisc.Text);
                UserGroupInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                UserGroupInfo.SystemName = Environment.MachineName;
                UserGroupInfo.UserID = Global.gblUserID;
                UserGroupInfo.LastUpdateDate = DateTime.Today;
                UserGroupInfo.LastUpdateTime = DateTime.Now;
                strRet = clsuser.InsertUpdateDeleteUserGroup(UserGroupInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                            MessageBox.Show("Hey! There are entries Associated with this UserGroupName [" + txtUserName.Text + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ClearAll();
                if (bFromEditWindowUserGp == true)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            try
            {
                txtUserName.Clear();
                txtbilldisc.Text = "0";
                txtItemdisc.Text = "0";
                txtCashdisc.Text = "0";
                strAccessLevelsettings = "";
                strAccessLevelTrans = "";
                strAccessLevelReport = "";
                strAccessLevelAcc = "";
                strAccessLevel = "";
                txtUserName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}
