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
using Syncfusion.Windows.Forms.Tools;
using DigiposZen.Forms;
using System.Runtime.InteropServices;

namespace DigiposZen
{
    // ======================================================== >>
    // Description:Employee Creation
    // Developed By:Anjitha K K
    // Completed Date & Time: 15/03/2022 5.30 PM
    // Last Edited By:
    // Last Edited Date & Time:
    // ======================================================== >>
    public partial class frmEmployee : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmEmployee(int iEmpID = 0, bool bFromEdit = false)
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

                bFromEditWindowEmployee = bFromEdit;
                iIDFromEditWindow = iEmpID;
                this.BackColor = Global.gblFormBorderColor;
                if (iIDFromEditWindow != 0)
                {
                    LoadDesignationFromOneTimeMaster();
                    LoadData(iIDFromEditWindow);
                }
                else
                {
                    btnDelete.Enabled = false;
                }
                txtName.Focus();
                txtName.SelectAll();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Employee" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region "VARIABLES -------------------------------------------- >>"
        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspEmployeeInsertInfo EmpInfo = new UspEmployeeInsertInfo();
        UspGetOnetimeMasterInfo GetOtminfo = new UspGetOnetimeMasterInfo();

        clsEmployee clsEmp = new clsEmployee();
        clsOneTimeMaster clsOtm = new clsOneTimeMaster();
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int intLID;
        int intCommission;
        int intPOstAccounts;
        int iIDFromEditWindow;
        int iAction = 0;
        string strCheck = "";
        bool bFromEditWindowEmployee;
        Control ctrl;
        int iActive = 0;
        int iActiveStaffLedger = 0;
        int iActiveSalesman = 0;
        string strDesigText;
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
        private void txtName_Click(object sender, EventArgs e)
        {
            toolEmployee.SetToolTip(txtName, "Please Specify Employee Name");
        }
        private void txtEmployeeCode_Click(object sender, EventArgs e)
        {
            toolEmployee.SetToolTip(txtEmployeeCode, "Please Specify Employee Code");
        }
        private void txtAddress_Click(object sender, EventArgs e)
        {
            toolEmployee.SetToolTip(txtAddress, "Please Specify Address");
        }
        private void txtPhone_Click(object sender, EventArgs e)
        {
            toolEmployee.SetToolTip(txtPhone, "Please Specify Phone Number");
        }
        private void txtEnrollNo_Click(object sender, EventArgs e)
        {
            toolEmployee.SetToolTip(txtEnrollNo, "Please Specify Enroll Number");
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
                        //SendKeys.Send("{F4}");
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
        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up)
                {
                    this.SelectNextControl(ctrl, false, false, false, false);
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    txtEmployeeCode.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press is not working properly ? " + "\n" + ex.Message + " | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void togglebtnActive_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    togglebtnSalesman.Focus();
                }
                else if (e.KeyCode == Keys.Enter )
                {
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press is not working properly ? " + "\n" + ex.Message + " | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    txtEmployeeCode.Focus();
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    if (Comm.IsCursorOnEmptyLine(txtAddress) == true)
                    {
                        e.SuppressKeyPress = true;
                        txtPhone.Focus();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press is not working properly ? " + "\n" + ex.Message + " | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void cmbDesig_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    txtPhone.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    txtEnrollNo.Focus();
                    cmbDesig.Text = strDesigText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Key Press is not working properly ? " + "\n" + ex.Message + " | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar.ToString() != "+" && e.KeyChar.ToString() != ",";
            //e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        //Casing And Selection color
        private void txtName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtName, true);
        }
        private void txtName_Leave(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (txtName.Text.Length > 7)
                {
                    if (txtEmployeeCode.Text.Trim() == "")
                        txtEmployeeCode.Text = txtName.Text;
                            //txtEmployeeCode.AppendText(txtName.Text.Substring(0, 7));
                }
                else
                {
                    if (txtEmployeeCode.Text.Trim() == "")
                        txtEmployeeCode.Text = txtName.Text;
                    //txtEmployeeCode.AppendText(txtName.Text);
                }
                Comm.ControlEnterLeave(txtName,false,true);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to  append Employee Name to Employee Code...." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Cursor.Current = Cursors.Default;
        }
        private void txtEmployeeCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEmployeeCode, true);
        }
        private void txtEmployeeCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEmployeeCode, false, true);
        }
        private void txtAddress_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress, true);
        }
        private void txtAddress_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress, false, true);
        }
        private void txtPhone_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPhone, true);
        }
        private void txtPhone_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPhone, false, true);
        }
        private void txtEnrollNo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEnrollNo, true);
        }
        private void txtEnrollNo_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtEnrollNo);
        }
        private void togglebtnbudget_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(togglebtnStaff, true);
        }
        private void togglebtnbudget_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(togglebtnStaff);
        }
        private void togglebtnSalesman_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(togglebtnSalesman, true);
        }
        private void togglebtnSalesman_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(togglebtnSalesman);
        }
        private void togglebtnActive_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(togglebtnActive, true);
        }
        private void togglebtnActive_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(togglebtnActive);
        }
        private void cmbDesig_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cmbDesig, true);
        }
        private void cmbDesig_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cmbDesig);
        }

        private void frmEmployee_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                {
                    Application.DoEvents();
                    LoadDesignationFromOneTimeMaster(0, "DESIGNATION");
                    cmbDesig.SelectedIndex = 0;
                }
                txtName.Focus();
                txtName.Select();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load Employee  ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmEmployee_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtName.Text != "")
                    {
                        if (txtName.Text != strCheck)
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
                //    frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent, "FRMEMPLOYEE");
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
                        if (bFromEditWindowEmployee == true)
                        {
                            if (iIDFromEditWindow > 5)
                            {
                                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Employee[" + txtName.Text + "] Permanently ?", Global.gblMessageCaption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                                if (dlgResult == DialogResult.Yes)
                                {
                                    try
                                    {
                                        DeleteData();

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    finally
                                    {
                                    }
                                }
                            }
                            else
                                MessageBox.Show("Default Employee [" + txtName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    
        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                SaveData();
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
            Cursor.Current = Cursors.Default;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            if (iIDFromEditWindow > 5)
            {
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Employee[" + txtName.Text + "] Permanently ?", Global.gblMessageCaption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    try
                    {
                        DeleteData();

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
            }
            else
                MessageBox.Show("Default Employee [" + txtName.Text + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (txtName.Text != "")
            {
                if (txtName.Text != strCheck)
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
            if (txtName.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Employee Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtName.Focus();
            }
            else if(txtEmployeeCode.Text.Trim() == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter Employee Code", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtEmployeeCode.Focus();
            }
            return bValidate;
        }
        //Description : Fill Designation From Table
        private void LoadDesignationFromOneTimeMaster(int iSelectedID = 0, string sOtmType = "")
        {
            DataTable dtOtm = new DataTable();
            GetOtminfo.OtmID = iSelectedID;
            GetOtminfo.OtmType = sOtmType.ToUpper();
            GetOtminfo.TenantID = Global.gblTenantID;
            dtOtm = clsOtm.GetOnetimeMaster(GetOtminfo);
            if (dtOtm.Rows.Count > 0)
            {
                Comm.LoadControl(cmbDesig, dtOtm, "", false, false, "OtmData", "OtmID");
            }
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            GetEmpInfo.EmpID = iSelectedID;
            GetEmpInfo.TenantID = Global.gblTenantID;
            //EmpInfo.blnSalesStaff=
            dtLoad = clsEmp.GetEmployee(GetEmpInfo);
            if (dtLoad.Rows.Count > 0)
            {
                txtName.Text = dtLoad.Rows[0]["Name"].ToString();
                strCheck = dtLoad.Rows[0]["Name"].ToString();
                txtEmployeeCode.Text = dtLoad.Rows[0]["EmpCode"].ToString();
                txtAddress.Text = dtLoad.Rows[0]["Address"].ToString();
                txtPhone.Text = dtLoad.Rows[0]["PhNo"].ToString();
                cmbDesig.Text = dtLoad.Rows[0]["Designation"].ToString();
                txtEnrollNo.Text = dtLoad.Rows[0]["EnrollNo"].ToString();

                if (Convert.ToDecimal(dtLoad.Rows[0]["blnSalesStaff"].ToString()) == 1)
                    togglebtnStaff.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else
                    togglebtnStaff.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                if (Convert.ToDecimal(dtLoad.Rows[0]["blnStatus"].ToString()) == 1)
                    togglebtnSalesman.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else
                    togglebtnSalesman.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                if (Convert.ToInt32(dtLoad.Rows[0]["Active"].ToString()) == 1)
                    togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else
                    togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

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
                    EmpInfo.EmpID = Comm.gfnGetNextSerialNo("tblEmployee", "EmpID");
                    if (EmpInfo.EmpID < 6)
                        EmpInfo.EmpID = 6;
                }
                else
                    EmpInfo.EmpID = iIDFromEditWindow;
                EmpInfo.Name =txtName.Text.TrimStart().TrimEnd();
                EmpInfo.Address = txtAddress.Text.TrimStart().TrimEnd();
                EmpInfo.NameOfFather = "";
                EmpInfo.PhNo =txtPhone.Text;
                EmpInfo.MaritialStatus = "";
                EmpInfo.NoOfFamilyMembers = "";
                EmpInfo.NameOFNominee = "";
                EmpInfo.Spouse = "";
                EmpInfo.SpouseEmployed = false;
                EmpInfo.OwnerOfResidence = false;
                EmpInfo.PANNo = "";
                EmpInfo.BloodGroup = "";
                EmpInfo.Designation = cmbDesig.Text.TrimStart().TrimEnd();
                EmpInfo.Qualification = "";
                EmpInfo.Sex = "";
                EmpInfo.DOB = Convert.ToDateTime("01-01-1900");
                EmpInfo.DOJ = Convert.ToDateTime("01-01-1900");
                EmpInfo.DOI = Convert.ToDateTime("01-01-1900");
                EmpInfo.PensionAccNo = "";
                EmpInfo.GPFAccNo = "";
                EmpInfo.GSLIAccNo = "";
                EmpInfo.LICPolicyNo = "";
                EmpInfo.LICMonthlyPremium = 0;
                EmpInfo.LICDateofMaturity = Convert.ToDateTime("01-01-1900");
                EmpInfo.CategoryID = 0;
                EmpInfo.DateofPromotion = Convert.ToDateTime("01-01-1900");
                EmpInfo.DateofRetirement = Convert.ToDateTime("01-01-1900");
                EmpInfo.GISAccNo = "";
                EmpInfo.BankAccNo = "";
                EmpInfo.Commission = 0;
                EmpInfo.CommissionAmt = 0;
                EmpInfo.EmpFname = "";
                if (togglebtnSalesman.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActiveSalesman = 1;
                else
                    iActiveSalesman = 0;
                EmpInfo.blnSalesStaff = iActiveSalesman;
                EmpInfo.PhotoPath = "";
                EmpInfo.InsCompany = "";
                EmpInfo.CommissionCondition = 0;
                EmpInfo.EmpCode = txtEmployeeCode.Text.TrimStart().TrimEnd();
                if (togglebtnStaff.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActiveStaffLedger = 1;
                else
                    iActiveStaffLedger = 0;
                EmpInfo.blnStatus = iActiveStaffLedger;
                EmpInfo.DrivingLicenceNo = "";
                EmpInfo.DrivingLicenceExpiry = Convert.ToDateTime("01-01-1900");
                EmpInfo.PassportNo = "";
                EmpInfo.PassportExpiry = Convert.ToDateTime("01-01-1900");
                if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActive = 1;
                else
                    iActive = 0;
                EmpInfo.Active = iActive;
                EmpInfo.SortOrder = 0;
                if (txtEnrollNo.Text == "")
                    txtEnrollNo.Text = "0";
                EmpInfo.EnrollNo = Convert.ToDecimal(txtEnrollNo.Text);
                EmpInfo.TargetAmount = 0;
                EmpInfo.IncentivePer = 0;
                EmpInfo.PWD = "";
                EmpInfo.Holidays = "";
                EmpInfo.LID = 0;
                EmpInfo.salarypermonth = 0;
                EmpInfo.SystemName = Environment.MachineName;
                EmpInfo.UserID = Global.gblUserID;
                EmpInfo.LastUpdateDate = DateTime.Today;
                EmpInfo.LastUpdateTime = DateTime.Now;
                EmpInfo.TenantID = Global.gblTenantID;
                strRet = clsEmp.InsertUpdateDeleteEmployee(EmpInfo, iAction);
                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            if (strResult[1].ToString().Contains("UK_EmpCode"))
                            {
                                MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the Employee Code (" + txtEmployeeCode.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                txtEmployeeCode.Focus();
                                txtEmployeeCode.SelectAll();
                            }
                            else
                            {
                                MessageBox.Show("Duplicate Entry ,  User has restricted to enter duplicate values in the Employee Name (" + txtName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtName.Focus();
                                txtName.SelectAll();
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
                    else
                    {
                        Comm.MessageboxToasted("Employee", "Employee saved successfully");
                        ClearAll();
                    }
                    Comm.MessageboxToasted("Employee", "Employee saved successfully");
                    if (bFromEditWindowEmployee == true)
                    {
                        this.Close();
                    }
                }
            }
        }
        //Description :  Delete Data from Employee table
        private void DeleteData()
        {
            string[] strResult;
            string strRet = "";
            iAction = 2;
            EmpInfo.EmpID = iIDFromEditWindow;
            EmpInfo.Name = txtName.Text;
            EmpInfo.Address = txtAddress.Text;
            EmpInfo.NameOfFather = "";
            EmpInfo.PhNo = txtPhone.Text;
            EmpInfo.MaritialStatus = "";
            EmpInfo.NoOfFamilyMembers = "";
            EmpInfo.NameOFNominee = "";
            EmpInfo.Spouse = "";
            EmpInfo.SpouseEmployed = false;
            EmpInfo.OwnerOfResidence = false;
            EmpInfo.PANNo = "";
            EmpInfo.BloodGroup = "";
            EmpInfo.Designation = Convert.ToString(cmbDesig.Text);
            EmpInfo.Qualification = "";
            EmpInfo.Sex = "";
            EmpInfo.DOB = Convert.ToDateTime("01-01-1900");
            EmpInfo.DOJ = Convert.ToDateTime("01-01-1900");
            EmpInfo.DOI = Convert.ToDateTime("01-01-1900");
            EmpInfo.PensionAccNo = "";
            EmpInfo.GPFAccNo = "";
            EmpInfo.GSLIAccNo = "";
            EmpInfo.LICPolicyNo = "";
            EmpInfo.LICMonthlyPremium = 0;
            EmpInfo.LICDateofMaturity = Convert.ToDateTime("01-01-1900");
            EmpInfo.CategoryID = 0;
            EmpInfo.DateofPromotion = Convert.ToDateTime("01-01-1900");
            EmpInfo.DateofRetirement = Convert.ToDateTime("01-01-1900");
            EmpInfo.GISAccNo = "";
            EmpInfo.BankAccNo = "";
            EmpInfo.Commission = 0;
            EmpInfo.CommissionAmt = 0;
            EmpInfo.EmpFname = "";
            if (togglebtnSalesman.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActiveSalesman = 1;
            else
                iActiveSalesman = 0;
            EmpInfo.blnSalesStaff = iActiveSalesman;
            EmpInfo.PhotoPath = "";
            EmpInfo.InsCompany = "";
            EmpInfo.CommissionCondition = 0;
            EmpInfo.EmpCode = txtEmployeeCode.Text;
            if (togglebtnStaff.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActiveStaffLedger = 1;
            else
                iActiveStaffLedger = 0;
            EmpInfo.blnStatus = iActiveStaffLedger;
            EmpInfo.DrivingLicenceNo = "";
            EmpInfo.DrivingLicenceExpiry = Convert.ToDateTime("01-01-1900");
            EmpInfo.PassportNo = "";
            EmpInfo.PassportExpiry = Convert.ToDateTime("01-01-1900");
            if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActive = 1;
            else
                iActive = 0;
            EmpInfo.Active = iActive;
            EmpInfo.SortOrder = 0;
            EmpInfo.EnrollNo = Convert.ToDecimal(txtEnrollNo.Text);
            EmpInfo.TargetAmount = 0;
            EmpInfo.IncentivePer = 0;
            EmpInfo.PWD = "";
            EmpInfo.Holidays = "";
            EmpInfo.LID = 0;
            EmpInfo.salarypermonth = 0;
            EmpInfo.SystemName = Environment.MachineName;
            EmpInfo.UserID = Global.gblUserID;
            EmpInfo.LastUpdateDate = DateTime.Today;
            EmpInfo.LastUpdateTime = DateTime.Now;
            EmpInfo.TenantID = Global.gblTenantID;
            strRet = clsEmp.InsertUpdateDeleteEmployee(EmpInfo, iAction);

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
            if (bFromEditWindowEmployee == true)
            {
                this.Close();
            }
        }
        //Description : Clear Data from Form
        private void ClearAll()
        {
            txtName.Text = "";
            txtEmployeeCode.Text = "";
            txtAddress.Text = "";
            txtPhone.Text = "";
            cmbDesig.SelectedIndex =0;
            txtEnrollNo.Text = "";
            togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            togglebtnStaff.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            togglebtnSalesman.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            txtName.Focus();

            GetOtminfo = new UspGetOnetimeMasterInfo();
            clsOtm = new clsOneTimeMaster();
            LoadDesignationFromOneTimeMaster(0, "DESIGNATION");

        }

        #endregion

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void rdoSettings_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmEmployee_Activated(object sender, EventArgs e)
        {
            try
            {

            }
            catch
            { }
        }

        private void picBackground_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.BackgroundImage = (Bitmap)picBackground.Image.Clone();
            }
            catch
            { }
        }
    }
}   
