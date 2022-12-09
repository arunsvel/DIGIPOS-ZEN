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
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace DigiposZen
{
    // ======================================================== >>
    // Description:Color Creation
    // Developed By:Pramod Philip
    // Completed Date & Time: 09/09/2021 3.30 PM
    // Last Edited By:Anjitha k k
    // Last Edited Date & Time:01-March-2022 02:30 PM
    // ======================================================== >>

    public partial class frmHSN : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmHSN(int iHID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
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

                pnlSlab.Enabled = false;

                //Comm.LoadBGImage(this, picBackground);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblSave.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblDelete.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblFind.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
                btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
                btnFind.Image = global::DigiposZen.Properties.Resources.find_finalised_3030;
                btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
                btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;

                cmbBTaxClass.Items.Clear();
                if (AppSettings.AVAILABLETAXPER == null)
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER == "")
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER != "")
                {
                    string[] str = AppSettings.AVAILABLETAXPER.Split(',');
                    bool isNumeric = false;
                    for (int i = 0; i < str.Length - 1; i++)
                    {
                        isNumeric = false;
                        isNumeric = int.TryParse(str[i], out int n);
                        if (isNumeric == true)
                        {
                            cmbBTaxClass.Items.Add(str[i]);
                        }
                    }
                }

                cmbBTaxClass1.Items.Clear();
                if (AppSettings.AVAILABLETAXPER == null)
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER == "")
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER != "")
                {
                    string[] str = AppSettings.AVAILABLETAXPER.Split(',');
                    bool isNumeric = false;
                    for (int i = 0; i < str.Length - 1; i++)
                    {
                        isNumeric = false;
                        isNumeric = int.TryParse(str[i], out int n);
                        if (isNumeric == true)
                        {
                            cmbBTaxClass1.Items.Add(str[i]);
                        }
                    }
                }

                cmbBTaxClass2.Items.Clear();
                if (AppSettings.AVAILABLETAXPER == null)
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER == "")
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER != "")
                {
                    string[] str = AppSettings.AVAILABLETAXPER.Split(',');
                    bool isNumeric = false;
                    for (int i = 0; i < str.Length - 1; i++)
                    {
                        isNumeric = false;
                        isNumeric = int.TryParse(str[i], out int n);
                        if (isNumeric == true)
                        {
                            cmbBTaxClass2.Items.Add(str[i]);
                        }
                    }
                }

                cmbBTaxClass3.Items.Clear();
                if (AppSettings.AVAILABLETAXPER == null)
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER == "")
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER != "")
                {
                    string[] str = AppSettings.AVAILABLETAXPER.Split(',');
                    bool isNumeric = false;
                    for (int i = 0; i < str.Length - 1; i++)
                    {
                        isNumeric = false;
                        isNumeric = int.TryParse(str[i], out int n);
                        if (isNumeric == true)
                        {
                            cmbBTaxClass3.Items.Add(str[i]);
                        }
                    }
                }

                cmbBTaxClass4.Items.Clear();
                if (AppSettings.AVAILABLETAXPER == null)
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER == "")
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER != "")
                {
                    string[] str = AppSettings.AVAILABLETAXPER.Split(',');
                    bool isNumeric = false;
                    for (int i = 0; i < str.Length - 1; i++)
                    {
                        isNumeric = false;
                        isNumeric = int.TryParse(str[i], out int n);
                        if (isNumeric == true)
                        {
                            cmbBTaxClass4.Items.Add(str[i]);
                        }
                    }
                }

                cmbHsnType.SelectedIndex = 0;

                CtrlPassed = Controlpassed;
                if (CtrlPassed != null && iIDFromEditWindow == 0)
                {
                    txtHSNCode.Text = CtrlPassed.Text.ToString();
                }
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

            iIDFromEditWindow = iHID;
            bFromEditWindowColor = bFromEdit;
         
            this.BackColor = Global.gblFormBorderColor;
            if (iHID != 0)
            {
                LoadData(iHID);
            }
            else
            {
                btnDelete.Enabled = false;
            }
            txtHSNCode.Focus();
            //txtHSNCode.SelectAll();

            txtHSNCode.SelectionStart = txtHSNCode.Text.Length;
            txtHSNCode.SelectionLength = 0;

            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspInsertHSNmasterInfo HSNmasterInfo = new UspInsertHSNmasterInfo();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        UspGetHSNInfo GetHSN = new UspGetHSNInfo();
        clsHSNMaster clsHSN = new clsHSNMaster();
        UspGetMasterInfo GetMaster = new UspGetMasterInfo();
        clsMaster clsMaster = new clsMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        string strCheck;
        Control ctrl;
        bool bFromEditWindowColor;
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
        private void txtColorName_Click(object sender, EventArgs e)
        {
            toolTipColor.SetToolTip(txtHSNCode, "Please specify the unique  Color");
        }
        private void txtColorHexCode_Click(object sender, EventArgs e)
        {
            toolTipColor.SetToolTip(txtDescription, "Please enter the  matched the Color HexCode");
        }

        private void frmHSN_Load(object sender, EventArgs e)
        {
            try
            {
                   
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    cmbBTaxClass.SelectedIndex = 0;

                    cmbBTaxClass1.SelectedIndex = 0;
                    cmbBTaxClass2.SelectedIndex = 0;
                    cmbBTaxClass3.SelectedIndex = 0;
                    cmbBTaxClass4.SelectedIndex = 0;

                    this.Show();
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                }
                //txtHSNCode.Select();

                txtHSNCode.Focus();

                txtHSNCode.SelectionStart = txtHSNCode.Text.Length;
                txtHSNCode.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Color......" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Cursor.Current = Cursors.Default;
        }
        private void frmHSN_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtHSNCode.Text != "")
                    {
                        if (txtHSNCode.Text != strCheck)
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
                else if (e.KeyCode == Keys.F5)//Save
                {
                    if (IsValidate() == true)
                    {
                        Comm.ControlEnterLeave(txtHSNCode);
                        Application.DoEvents();

                        SaveData();
                    }
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowColor == true)
                    {
                        try
                        {
                            if (Convert.ToDecimal(iIDFromEditWindow) > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Color[" + txtHSNCode.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    DeleteData();
                                }
                            }
                            else
                                MessageBox.Show("Default Color [" + txtHSNCode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Shortcut keys not working properly  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    }
                }
                else
                {
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
            Cursor.Current = Cursors.Default;
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtColorHexCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtHSNCode.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtColorName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHSNCode, true);
        }
        private void txtColorName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHSNCode);
        }
        private void txtColorHexCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription, true);
        }
        private void txtColorHexCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription, false, false);
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
                MessageBox.Show("Failed to Save...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Color[" + txtHSNCode.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                }
                else
                    MessageBox.Show("Default Color [" + txtHSNCode.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed Delete...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (txtHSNCode.Text != "")
                {
                    if (txtHSNCode.Text != strCheck)
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
                MessageBox.Show("Failed to Close...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (txtHSNCode.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please enter HSN Code", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtHSNCode.Focus();
            }
            else
            {
                if (txtDescription.Text == "")
                    txtDescription.Text = "0";

                txtHSNCode.Text = txtHSNCode.Text.Replace("'", "\"");
            }

            if (pnlSlab.Enabled == true)
            {
                if (((Comm.ToDecimal(txtAmountBefore1.Text)) == 0) || ((Comm.ToDecimal(txtAmountAfter1.Text)) == 0))
                {
                    bValidate = false;
                    MessageBox.Show("First slab row should not have any zero or blank values. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    txtAmountBefore1.Focus();
                }
                if (((Comm.ToDecimal(txtAmountBefore1.Text)) != 0) & ((Comm.ToDecimal(txtAmountAfter1.Text)) != 0))
                {
                    if ((Comm.ToDecimal(txtAmountBefore1.Text)) >= (Comm.ToDecimal(txtAmountAfter1.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Please correct value mismatch in first slab. Amount 1 is greater than amount 2.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountBefore1.Focus();
                    }
                }

                if (((Comm.ToDecimal(txtAmountBefore2.Text)) == 0) ^ ((Comm.ToDecimal(txtAmountAfter2.Text)) == 0))
                {
                    bValidate = false;
                    MessageBox.Show("Found value mismatch in second slab. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    txtAmountBefore2.Focus();
                }
                if (((Comm.ToDecimal(txtAmountBefore2.Text)) != 0) & ((Comm.ToDecimal(txtAmountAfter2.Text)) != 0))
                {
                    if ((Comm.ToDecimal(txtAmountBefore2.Text)) >= (Comm.ToDecimal(txtAmountAfter2.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Please correct value mismatch in second slab. Amount 3 is greater than amount 4.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountBefore2.Focus();
                    }
                    if ((Comm.ToDecimal(txtAmountBefore2.Text)) != (Comm.ToDecimal(txtAmountAfter1.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Second amount in first slab row and first amount in second slab should be identical. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountAfter1.Focus();
                    }
                }

                if (((Comm.ToDecimal(txtAmountBefore3.Text)) == 0) ^ ((Comm.ToDecimal(txtAmountAfter3.Text)) == 0))
                {
                    bValidate = false;
                    MessageBox.Show("Found value mismatch in third slab. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    txtAmountBefore3.Focus();
                }
                if (((Comm.ToDecimal(txtAmountBefore3.Text)) != 0) & ((Comm.ToDecimal(txtAmountAfter3.Text)) != 0))
                {
                    if ((Comm.ToDecimal(txtAmountBefore3.Text)) >= (Comm.ToDecimal(txtAmountAfter3.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Please correct value mismatch in third slab. Amount 5 is greater than amount 6.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountBefore3.Focus();
                    }
                    if ((Comm.ToDecimal(txtAmountBefore3.Text)) != (Comm.ToDecimal(txtAmountAfter2.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Second amount in second slab row and first amount in third slab should be identical. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountAfter2.Focus();
                    }
                }

                if (((Comm.ToDecimal(txtAmountBefore4.Text)) == 0) ^ ((Comm.ToDecimal(txtAmountAfter4.Text)) == 0))
                {
                    bValidate = false;
                    MessageBox.Show("Found value mismatch in fourth slab. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    txtAmountBefore4.Focus();
                }
                if (((Comm.ToDecimal(txtAmountBefore4.Text)) != 0) & ((Comm.ToDecimal(txtAmountAfter4.Text)) != 0))
                {
                    if ((Comm.ToDecimal(txtAmountBefore4.Text)) >= (Comm.ToDecimal(txtAmountAfter4.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Please correct value mismatch in fourth slab. Amount 7 is greater than amount 8.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountBefore4.Focus();
                    }
                    if ((Comm.ToDecimal(txtAmountBefore4.Text)) != (Comm.ToDecimal(txtAmountAfter3.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("Second amount in third slab row and first amount in fourth slab should be identical. Please correct the values and try again.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtAmountAfter3.Focus();
                    }
                }
            }
            else
            {
                bValidate = true;
            }

            if (bValidate == false)
            {
                return false;
            }
            string a="select * from tblHSNCode where HSNCODE = '"+txtHSNCode.Text+"' and IGSTTaxPer = '"+cmbBTaxClass.Text+"'";
            SqlDataAdapter da1 = new SqlDataAdapter("select * from tblHSNCode where HSNCODE='"+txtHSNCode.Text+"'  and IGSTTaxPer='"+cmbBTaxClass.Text+"'", DigiposZen.Properties.Settings.Default.ConnectionString);
            DataTable dt3 = new DataTable();
            da1.Fill(dt3);


            if (dt3.Rows.Count > 0 && iIDFromEditWindow==0)
            {
                bValidate = false;
                MessageBox.Show("Duplicate Entry", "Error");

            }

            if (txtCess.Text == "" || txtCess.Text == null)//Nothing selected
            {
                txtCess.Text = "0";
            }

            if (txtCompCess.Text == "" || txtCompCess.Text == null)//Nothing selected
            {
                txtCompCess.Text = "0";
            }

            if (cmbBTaxClass.SelectedIndex == -1)//Nothing selected
            {
                bValidate = false;
                MessageBox.Show("You must select a Tax Class", "Error");
            }
           
            return bValidate;
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetHSN.HID = Convert.ToDecimal(iSelectedID);
            GetHSN.TenantID = Convert.ToDecimal(Global.gblTenantID);
            dtLoad = clsHSN.GetHSNMaster(GetHSN);
            if (dtLoad.Rows.Count > 0)
            {
                txtHSNCode.Text = dtLoad.Rows[0]["HSNCODE"].ToString();
                txtDescription.Text = dtLoad.Rows[0]["HSNDECRIPTION"].ToString();

                cmbBTaxClass.Text = dtLoad.Rows[0]["IGSTTaxPer"].ToString();
                txtCess.Text= dtLoad.Rows[0]["CessPer"].ToString();
                txtCompCess.Text= dtLoad.Rows[0]["CompCessQty"].ToString();
                cmbHsnType.Text= dtLoad.Rows[0]["HSNType"].ToString();

                if (dtLoad.Rows[0]["blnSlabSystem"].ToString() == "1.00")
                {
                    chkBSlabEnabled1.Checked = true;

                    txtAmountAfter1.Text = dtLoad.Rows[0]["ValueEndSB1"].ToString();
                    txtAmountAfter2.Text = dtLoad.Rows[0]["ValueEndSB2"].ToString();
                    txtAmountAfter3.Text = dtLoad.Rows[0]["ValueEndSB3"].ToString();
                    txtAmountAfter4.Text = dtLoad.Rows[0]["ValueEndSB4"].ToString();
                    txtAmountBefore1.Text = dtLoad.Rows[0]["ValueStartSB1"].ToString();
                    txtAmountBefore2.Text = dtLoad.Rows[0]["ValueStartSB2"].ToString();
                    txtAmountBefore3.Text = dtLoad.Rows[0]["ValueStartSB3"].ToString();
                    txtAmountBefore4.Text = dtLoad.Rows[0]["ValueStartSB4"].ToString();
                    cmbBTaxClass1.Text = dtLoad.Rows[0]["IGSTTaxPer1"].ToString();
                    cmbBTaxClass2.Text = dtLoad.Rows[0]["IGSTTaxPer2"].ToString();
                    cmbBTaxClass3.Text = dtLoad.Rows[0]["IGSTTaxPer3"].ToString();
                    cmbBTaxClass4.Text = dtLoad.Rows[0]["IGSTTaxPer4"].ToString();
                }

            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                if (iIDFromEditWindow != 0)
                {
                    iAction = 1;
                }
                string[] strResult;
                string strRet = "";
                if (iAction == 0)
                {
                    HSNmasterInfo.HID = Comm.gfnGetNextSerialNo("tblHSNCode", "HID");
                    if (HSNmasterInfo.HID < 6)
                        HSNmasterInfo.HID = 6;
                }
                else
                    HSNmasterInfo.HID = Convert.ToDecimal(iIDFromEditWindow);
                HSNmasterInfo.HSNCODE = txtHSNCode.Text;
                DataTable dtUspColor = new DataTable();
                HSNmasterInfo.HSNDECRIPTION = txtDescription.Text;
                HSNmasterInfo.HSNType = cmbHsnType.Text;
                HSNmasterInfo.IGSTTaxPer = Convert.ToDecimal(cmbBTaxClass.Text);
                HSNmasterInfo.CGSTTaxPer = Convert.ToDecimal(cmbBTaxClass.Text) / 2;
                HSNmasterInfo.SGSTTaxPer = Convert.ToDecimal(cmbBTaxClass.Text) / 2;

                if (chkBSlabEnabled1.Checked == true)
                { 
                    HSNmasterInfo.blnSlabSystem = 1;


                    if (txtAmountAfter1.Text != "")
                    {
                        HSNmasterInfo.ValueStartSB1 = Convert.ToDecimal(txtAmountBefore1.Text);
                        HSNmasterInfo.ValueEndSB1 = Convert.ToDecimal(txtAmountAfter1.Text);
                        HSNmasterInfo.IGSTTaxPer1 = Convert.ToDecimal(cmbBTaxClass1.Text);
                        HSNmasterInfo.CGSTTaxPer1 = Convert.ToDecimal(cmbBTaxClass1.Text) / 2;
                        HSNmasterInfo.SGSTTaxPer1 = Convert.ToDecimal(cmbBTaxClass1.Text) / 2;
                    }
                    if (txtAmountAfter2.Text != "")
                    {
                        HSNmasterInfo.ValueStartSB2 = Convert.ToDecimal(txtAmountBefore2.Text);
                        HSNmasterInfo.ValueEndSB2 = Convert.ToDecimal(txtAmountAfter2.Text);
                        HSNmasterInfo.IGSTTaxPer2 = Convert.ToDecimal(cmbBTaxClass2.Text);
                        HSNmasterInfo.CGSTTaxPer2 = Convert.ToDecimal(cmbBTaxClass2.Text) / 2;
                        HSNmasterInfo.SGSTTaxPer2 = Convert.ToDecimal(cmbBTaxClass2.Text) / 2;
                    }
                    if (txtAmountAfter3.Text != "")
                    {
                        HSNmasterInfo.ValueStartSB3 = Convert.ToDecimal(txtAmountBefore3.Text);
                        HSNmasterInfo.ValueEndSB3 = Convert.ToDecimal(txtAmountAfter3.Text);
                        HSNmasterInfo.IGSTTaxPer3 = Convert.ToDecimal(cmbBTaxClass3.Text);
                        HSNmasterInfo.CGSTTaxPer3 = Convert.ToDecimal(cmbBTaxClass3.Text) / 2;
                        HSNmasterInfo.SGSTTaxPer3 = Convert.ToDecimal(cmbBTaxClass3.Text) / 2;
                    }
                    if (txtAmountAfter4.Text != "")
                    {
                        HSNmasterInfo.ValueEndSB4 = Convert.ToDecimal(txtAmountAfter4.Text);
                        HSNmasterInfo.ValueStartSB4 = Convert.ToDecimal(txtAmountBefore4.Text);
                        HSNmasterInfo.IGSTTaxPer4 = Convert.ToDecimal(cmbBTaxClass4.Text);
                        HSNmasterInfo.CGSTTaxPer4 = Convert.ToDecimal(cmbBTaxClass4.Text) / 2;
                        HSNmasterInfo.SGSTTaxPer4 = Convert.ToDecimal(cmbBTaxClass4.Text) / 2;
                    }
                }
                HSNmasterInfo.CessPer = Convert.ToDecimal(txtCess.Text);
                HSNmasterInfo.CompCessQty = Convert.ToDecimal(txtCompCess.Text);

             
                HSNmasterInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                strRet = clsHSN.InsertUpdateDeleteHSNMaster(HSNmasterInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            MessageBox.Show("Duplicate Entry , User has restricted to enter duplicate values in the HSNCODE (" + txtHSNCode.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtHSNCode.Focus();
                            //txtHSNCode.SelectAll();
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        CtrlPassed.Text = txtHSNCode.Text;
                        CtrlPassed.Tag = HSNmasterInfo.HID;

                        CtrlPassed.Focus();
                        this.Close();
                    }
                    else
                    {
                        ClearAll();
                        if (bFromEditWindowColor == true)
                        {
                            this.Close();
                        }

                    }
                    Comm.MessageboxToasted("HSN Code", "HSN Code saved successfully");
                }
            }
        }
        //Description :  Delete Data from Color table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            DataTable dtUspColor = new DataTable();
            HSNmasterInfo.HID = Convert.ToDecimal(iIDFromEditWindow);
            HSNmasterInfo.HSNCODE = txtHSNCode.Text;
            DataTable dtMaster = new DataTable();
            HSNmasterInfo.HSNDECRIPTION = txtDescription.Text;
            HSNmasterInfo.HSNType = cmbHsnType.Text;
            HSNmasterInfo.IGSTTaxPer = Convert.ToDecimal(cmbBTaxClass.Text);
            HSNmasterInfo.CGSTTaxPer = Convert.ToDecimal(cmbBTaxClass.Text) / 2;
            HSNmasterInfo.SGSTTaxPer = Convert.ToDecimal(cmbBTaxClass.Text) / 2;

            if (chkBSlabEnabled1.Checked == true)
            {
                HSNmasterInfo.blnSlabSystem = 1;


                if (txtAmountAfter1.Text != "")
                {
                    HSNmasterInfo.ValueStartSB1 = Convert.ToDecimal(txtAmountBefore1.Text);
                    HSNmasterInfo.ValueEndSB1 = Convert.ToDecimal(txtAmountAfter1.Text);
                    HSNmasterInfo.IGSTTaxPer1 = Convert.ToDecimal(cmbBTaxClass1.Text);
                    HSNmasterInfo.CGSTTaxPer1 = Convert.ToDecimal(cmbBTaxClass1.Text) / 2;
                    HSNmasterInfo.SGSTTaxPer1 = Convert.ToDecimal(cmbBTaxClass1.Text) / 2;
                }
                if (txtAmountAfter2.Text != "")
                {
                    HSNmasterInfo.ValueStartSB2 = Convert.ToDecimal(txtAmountBefore2.Text);
                    HSNmasterInfo.ValueEndSB2 = Convert.ToDecimal(txtAmountAfter2.Text);
                    HSNmasterInfo.IGSTTaxPer2 = Convert.ToDecimal(cmbBTaxClass2.Text);
                    HSNmasterInfo.CGSTTaxPer2 = Convert.ToDecimal(cmbBTaxClass2.Text) / 2;
                    HSNmasterInfo.SGSTTaxPer2 = Convert.ToDecimal(cmbBTaxClass2.Text) / 2;
                }
                if (txtAmountAfter3.Text != "")
                {
                    HSNmasterInfo.ValueStartSB3 = Convert.ToDecimal(txtAmountBefore3.Text);
                    HSNmasterInfo.ValueEndSB3 = Convert.ToDecimal(txtAmountAfter3.Text);
                    HSNmasterInfo.IGSTTaxPer3 = Convert.ToDecimal(cmbBTaxClass3.Text);
                    HSNmasterInfo.CGSTTaxPer3 = Convert.ToDecimal(cmbBTaxClass3.Text) / 2;
                    HSNmasterInfo.SGSTTaxPer3 = Convert.ToDecimal(cmbBTaxClass3.Text) / 2;
                }
                if (txtAmountAfter4.Text != "")
                {
                    HSNmasterInfo.ValueEndSB4 = Convert.ToDecimal(txtAmountAfter4.Text);
                    HSNmasterInfo.ValueStartSB4 = Convert.ToDecimal(txtAmountBefore4.Text);
                    HSNmasterInfo.IGSTTaxPer4 = Convert.ToDecimal(cmbBTaxClass4.Text);
                    HSNmasterInfo.CGSTTaxPer4 = Convert.ToDecimal(cmbBTaxClass4.Text) / 2;
                    HSNmasterInfo.SGSTTaxPer4 = Convert.ToDecimal(cmbBTaxClass4.Text) / 2;
                }
            }
            HSNmasterInfo.CessPer = Convert.ToDecimal(txtCess.Text);
            HSNmasterInfo.CompCessQty = Convert.ToDecimal(txtCompCess.Text);


            HSNmasterInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
             dtMaster = clsMaster.GetColumnIDsData(GetMaster);//Checking Color is Used in Item Master or not
            if (dtMaster.Rows.Count == 0)
            {
                strRet = clsHSN.InsertUpdateDeleteHSNMaster(HSNmasterInfo, iAction);
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
                        MessageBox.Show("Failed to Save", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        ClearAll();
                }
                if (bFromEditWindowColor == true)
                {
                    this.Close();
                }
            }
            else
                MessageBox.Show("Hey! There are Items Associated with this Color [" + txtHSNCode.Text + "]. Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtHSNCode.Clear();
            txtDescription.Clear();
            btnDelete.Enabled = false;
            txtHSNCode.Focus();
        }
        #endregion


        private void chkBSlabEnabled1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBSlabEnabled1.Checked == true)
            {
                pnlSlab.Enabled = true;//For enabling all controls inside the panel.

                cmbBTaxClass.Enabled = false;//To disable Tax class combo box outside the Slab panel.
            }

            if (chkBSlabEnabled1.Checked == false)
            {
                pnlSlab.Enabled = false;//For disabling all controls inside the panel.

                cmbBTaxClass.Enabled = true;//To enable Tax class combo box outside the Slab panel.
            }
        }

      

        private void txtHSNCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbBTaxClass_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is ComboBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkBSlabEnabled1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is CheckBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountBefore1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountAfter1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbBTaxClass1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is ComboBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountBefore2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountAfter2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbBTaxClass2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is ComboBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountBefore3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountAfter3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbBTaxClass3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is ComboBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountBefore4_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmountAfter4_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbBTaxClass4_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ctrl = (Control)sender;
                if (ctrl is ComboBox)
                {
                    if ((e.Shift == true && e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Up))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Key enter is not working Properly" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtHSNCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        

        private void txtAmountBefore1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountAfter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountBefore2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountAfter2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountBefore3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountAfter3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountBefore4_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountAfter4_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtAmountBefore1_Validating(object sender, CancelEventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtCompCess_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

       
    }
}
