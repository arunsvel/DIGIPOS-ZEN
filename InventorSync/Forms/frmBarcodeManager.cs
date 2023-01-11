using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.Info;
using DigiposZen.InventorBL.Helper;
using DigiposZen.InventorBL.Master;
using DigiposZen.JsonClass;

namespace DigiposZen.Forms
{
    public partial class frmBarcodeManager : Form, IMessageFilter
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        Common Comm = new Common();
        clsJSonCommon JSonComm = new clsJSonCommon();

        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspGetCostCentreInfo GetCctinfo = new UspGetCostCentreInfo();
        UspGetItemMasterInfo GetItmMstInfo = new UspGetItemMasterInfo();
        UspGetItemMasterFromStockInfo GetItmMststockinfo = new UspGetItemMasterFromStockInfo();

        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsItemMaster clsItmMst = new clsItemMaster();

        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsGetStockInVoucherSettings clsVchTypeFeatures = new clsGetStockInVoucherSettings();

        bool bFromEditBarCodeMgr = false;

        int iIDFromEditWindow = 0;
        int vchtypeID = 0;

        public frmBarcodeManager(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
        {
            try
            {
                InitializeComponent();

                controlsToMove.Add(this);
                controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

                frmMDI form = (frmMDI)MDIParent;
                this.MdiParent = form;
                int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
                int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
                this.SetBounds(5, 0, l, t);

                cmbDisplayStyle.SelectedIndex = 0;

                clsVchType = JSonComm.GetVoucherType(iVchTpeId);
                clsVchTypeFeatures = JSonComm.GetVoucherTypeGeneralSettings(iVchTpeId, 1);

                bFromEditBarCodeMgr = bFromEdit;
                iIDFromEditWindow = iTransID;
                vchtypeID = iVchTpeId;

                FillCostCentre();
                FillEmployee();

                if (iTransID != 0)
                {
                    //FillCostCentre();

                    //SetTransactionsthatVarying();

                    iIDFromEditWindow = Convert.ToInt32(iTransID);

                    //txtInvAutoNo.Select();
                }
                //else
                //SetTransactionsthatVarying();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Variables
        Control ctrl;


        //Description : Get Employee Details from Database
        public DataTable GetEmployee(int iSelID = 0)
        {
            try
            {
                GetEmpInfo.EmpID = iSelID;
                GetEmpInfo.TenantID = Global.gblTenantID;
                GetEmpInfo.blnSalesStaff = true;
                return clsEmp.GetEmployee(GetEmpInfo);
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return new DataTable();
            }
            finally
            {
            }

        }

        //Description: Fill Employee Details from GetEmployee Method to Combobox
        private void FillEmployee(int iSelID = 0)
        {
            try
            {
                DataTable dtEmp = new DataTable();
                dtEmp = GetEmployee(0);
                if (dtEmp.Rows.Count > 0)
                {
                    Comm.LoadControl(cboSalesStaff, dtEmp, "", false, false, "Name", "EmpID");
                    if (iSelID != 0)
                        cboSalesStaff.SelectedValue = iSelID;
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

        }

        //Description: Get Cost Centre Details from the Database
        public DataTable GetCostCentre(int iSelID = 0)
        {
            try
            {
                GetCctinfo.CCID = iSelID;
                GetCctinfo.TenantID = Global.gblTenantID;
                return clscct.GetCostCentre(GetCctinfo);
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return new DataTable();
            }
            finally
            {
            } 
        }

        //Description: Fill CostCentre from Get CostCentre Method
        private void FillCostCentre(int iSelID = 0)
        {
            try
            {
                DataTable dtCct = new DataTable();
                dtCct = GetCostCentre(0);
                if (dtCct == null)
                {
                    if (dtCct.Rows.Count > 0)
                    {
                        cboCostCentre.DataSource = dtCct;
                        cboCostCentre.DisplayMember = "Cost Centre Name";
                        cboCostCentre.ValueMember = "CCID";
                        if (iSelID != 0)
                            cboCostCentre.SelectedValue = iSelID;
                    }
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

        }

        private void FetchInvNo()
        {
            try
            {
                if (clsVchType.TransactionPrefix != "") // Transactoin Prefix
                {
                    txtPrefix.Text = clsVchType.TransactionPrefix.Trim();
                    txtPrefix.Visible = true;
                }
                else
                    txtPrefix.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                {
                    if (iIDFromEditWindow == 0) //New
                    {
                        txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBarcodeManager", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                        txtInvAutoNo.Tag = 0;
                    }
                    txtInvAutoNo.ReadOnly = true;
                    txtPrefix.ReadOnly = true;
                }
                else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                {
                    if (iIDFromEditWindow == 0) //New
                    {
                        //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBarcodeManager", "AutoNum").ToString();
                        txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBarcodeManager", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                        txtInvAutoNo.Tag = 0;
                    }

                    txtInvAutoNo.ReadOnly = false;
                    txtPrefix.ReadOnly = false;
                }
                else
                {
                    txtInvAutoNo.Tag = 0;
                    txtInvAutoNo.ReadOnly = false;
                    txtPrefix.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Setting Default Transactional Settings to the form
        private void SetTransactionDefaults()
        {
            try
            {
                if (clsVchType == null)
                {
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", "Sales Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            FetchInvNo();

            try
            {
                if (clsVchType.blnSaleStaffLockWSel == 1)
                    cboSalesStaff.Enabled = false;
                else
                    cboSalesStaff.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Setting Transactions that Varying to the form
        private void SetTransactionsthatVarying()
        {
            try
            {
                if (clsVchType == null)
                {
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", "Sales Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                cboCostCentre.SelectedValue = Comm.ConvertI32(clsVchType.PrimaryCCValue);
                cboSalesStaff.SelectedValue = Comm.ConvertI32(clsVchType.DefaultSaleStaffValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Setting asper Application Settings
        private void SetApplicationSettings()
        {
            try
            {
                if (AppSettings.NeedCostCenter == true)
                {
                    lblCostCenter.Visible = true;
                    cboCostCentre.Visible = true;
                }
                else
                {
                    lblCostCenter.Visible = false;
                    cboCostCentre.Visible = true;
                }

                dtpInvDate.MinDate = AppSettings.FinYearStart;
                dtpInvDate.MaxDate = AppSettings.FinYearEnd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
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

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            try
            {
                this.WindowState = FormWindowState.Minimized;
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

        private void btnSearchFwd_Click(object sender, EventArgs e)
        {
            //DataGridViewCell Cell = Comm.Search(DgvData, txtSearch.Text.ToString(), true, chkMatchCase.CheckState, chkExactWordOnly.CheckState);
            //if (Cell != null)
            //    DgvData.CurrentCell = Cell;
        }

        private void btnSearchBwd_Click(object sender, EventArgs e)
        {
            //DataGridViewCell Cell = Comm.Search(DgvData, txtSearch.Text.ToString(), false, chkMatchCase.CheckState, chkExactWordOnly.CheckState);
            //if (Cell != null)
            //    DgvData.CurrentCell = Cell;
        }

        public bool PreFilterMessage(ref Message m)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void DgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnFillData_Click(object sender, EventArgs e)
        {
            try
            {
                string whereSQL = "";

                DgvData.DataSource = null;

                if (txtFillSearch.Text != "")
                    whereSQL = " and  dbo.tblItemMaster.ItemCode + dbo.tblItemMaster.ItemName + dbo.tblStock.BatchUnique like '%" + txtFillSearch.Text + "%' ";
                this.Cursor = Cursors.AppStarting;
                btnFillData.Enabled = false;

                string QuerySQL = "";

                switch (cmbDisplayStyle.Text.ToUpper())
                {
                    case "<ALL ITEMS>":
                        {
                            QuerySQL = "";
                            break;
                        }

                    case "ORPHAN BATCHES":
                        {
                            QuerySQL = " and  StockID in (SELECT  [StockID]   FROM  [tblStock]   where BatchUnique  not in (Select BatchUnique from tblStockHistory) ";
                            break;
                        }

                    case "NEGATIVE /ZERO QTY BATCHES":
                        {
                            QuerySQL = " and isnull(tblStock.qty,0) <= 0 ";
                            break;
                        }

                    case "ACTIVE BATCHES":
                        {
                            QuerySQL = " and isnull(tblStock.StockActiveStatus, 0) = 1 ";
                            break;
                        }

                    case "DEACTIVE BATCHES":
                        {
                            QuerySQL = " and isnull(tblStock.StockActiveStatus, 1) = 0 ";
                            break;
                        }
                }

                string SQL = @" SELECT   dbo.tblItemMaster.ItemID, dbo.tblStock.StockID,  dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName, dbo.tblStock.BatchUnique,
                    CONVERT(DECIMAL(20," + AppSettings.QtyDecimals + "), dbo.tblStock.qoh) AS Qty, CONVERT(DECIMAL(20," + AppSettings.CurrencyDecimals + "), dbo.tblStock.Prate ) as Prate, CONVERT(DECIMAL(20," + AppSettings.CurrencyDecimals + "), dbo.tblStock.CostRateExcl ) as Crate,CONVERT(DECIMAL(20," + AppSettings.CurrencyDecimals + "), dbo.tblStock.MRP ) as MRP, isnull(tblStock.StockActiveStatus, 1) as OldStatus, isnull(tblStock.StockActiveStatus, 1) as ActiveStatus        FROM    dbo.tblItemMaster INNER JOIN    dbo.tblStock ON dbo.tblItemMaster.ItemID = dbo.tblStock.ItemID  WHERE  (dbo.tblItemMaster.ActiveStatus = 1) " + whereSQL + QuerySQL + "   ORDER BY dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName ";
                DgvData.Rows.Clear();
                DgvData.Columns.Clear();
                DgvData.DataSource = Comm.fnGetData(SQL).Tables[0];

                //loadcontrol(DgvData, SQL);

                if (DgvData.Rows.Count > 0)
                {
                    DgvData.Columns.Insert(DgvData.Columns.Count - 1, new DataGridViewCheckBoxColumn());

                    DgvData.Columns[0].Visible = false;
                    DgvData.Columns[1].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 1].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 3].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 2].HeaderText = "Active Status";
                    DgvData.Columns[DgvData.Columns.Count - 3].HeaderText = "Old Status";

                    int i = 0;
                    foreach (DataGridViewRow row in DgvData.Rows)
                    {
                        DgvData[Comm.ToInt32(DgvData.Columns.Count - 2), i].Value = Comm.ToInt32(DgvData[DgvData.Columns.Count - 1, i].Value) == 1 ? CheckState.Checked : CheckState.Unchecked;
                        i++;
                    }
                }
                // DgvData.SelectionMode = DataGridViewSelectionMode.FullColumnSelect
                DgvData.Focus();
                this.Cursor = Cursors.Default;
                DgvData.Cursor = Cursors.Default;

                btnFillData.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Barcode Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Cursor = Cursors.Default;
                btnFillData.Enabled = true;
                DgvData.Cursor = Cursors.Default;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                sqlControl rs = new sqlControl();

                try
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



                    rs.BeginTrans = true;

                    int invid;
                    if (iIDFromEditWindow == 0)
                    {
                        invid = Comm.gfnGetNextSerialNo("tblBarcodeManager", "invid");
                        FetchInvNo();
                    }
                    else
                    {
                        invid = Comm.ToInt32(txtInvAutoNo.Tag);
                    }

                    bool blnStartedInsert = false;

                    for (int i = 0; i < DgvData.RowCount; i++)
                    {
                        if ((Convert.ToBoolean(DgvData[Comm.ToInt32(DgvData.Columns.Count - 2), i].Value) == true && Comm.ToInt32(DgvData[Comm.ToInt32(DgvData.Columns.Count - 3), i].Value) != 1)
                            || (Convert.ToBoolean(DgvData[Comm.ToInt32(DgvData.Columns.Count - 2), i].Value) == false && Comm.ToInt32(DgvData[Comm.ToInt32(DgvData.Columns.Count - 3), i].Value) != 0))
                        {
                            if (blnStartedInsert == false)
                            {
                                //insert to table
                                rs.Execute("Insert Into tblBarcodeManager (InvID,AutoNum,InvNo,VchtypeID,Prefix,VchDate,CCID,StaffID) Values (" + invid + "," + txtInvAutoNo.Text.ToString() + ",'" + txtPrefix.Text.ToString() + txtInvAutoNo.Text.ToString() + "'," + vchtypeID + ",'" + txtPrefix.Text.ToString() + "','" + dtpInvDate.Value.ToString("dd/MMM/yyyy") + "'," + cboCostCentre.SelectedValue + "," + cboSalesStaff.SelectedValue + ")");
                                blnStartedInsert = true;
                            }
                            rs.Execute("Insert Into tblBarcodeManagerItemStatus (InvID,ItemID,StockID,BatchUnique,Qty,PRate,Crate,MRP,OldStatus,NewStatus) Values (" + invid + "," + DgvData[0, i].Value + "," + DgvData[1, i].Value + ",'" + DgvData[4, i].Value + "'," + DgvData[5, i].Value + "," + DgvData[6, i].Value + "," + DgvData[7, i].Value + "," + DgvData[8, i].Value + "," + (Convert.ToBoolean(DgvData[9, i].Value) == true ? 1 : 0) + "," + (Convert.ToBoolean(DgvData[10, i].Value) == true ? 1 : 0) + ")");

                            rs.Execute("UPDATE TBLSTOCK SET StockActiveStatus = " + (Convert.ToBoolean(DgvData[10, i].Value) == true ? 1 : 0) + " WHERE ITEMID=" + DgvData[0, i].Value + " AND BATCHUNIQUE='" + DgvData[4, i].Value + "' ");

                            //        string SQL = @" SELECT   dbo.tblItemMaster.ItemID, dbo.tblStock.StockID,  dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName, dbo.tblStock.BatchUnique,
                            //CONVERT(DECIMAL(20," + AppSettings.QtyDecimals + "), dbo.tblStock.qoh) AS Qty, CONVERT(DECIMAL(20," + AppSettings.CurrencyDecimals + "), dbo.tblStock.Prate ) as Prate, CONVERT(DECIMAL(20," + AppSettings.CurrencyDecimals + "), dbo.tblStock.CostRateExcl ) as Crate,CONVERT(DECIMAL(20," + AppSettings.CurrencyDecimals + "), dbo.tblStock.MRP ) as MRP, isnull(tblStock.StockActiveStatus, 1) as OldStatus, isnull(tblStock.StockActiveStatus, 1) as ActiveStatus        FROM    dbo.tblItemMaster INNER JOIN    dbo.tblStock ON dbo.tblItemMaster.ItemID = dbo.tblStock.ItemID  WHERE  (dbo.tblItemMaster.ActiveStatus = 1) " + whereSQL + QuerySQL + "   ORDER BY dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName ";
                        }
                    }
                    rs.CommitTrans = true;

                    ClearControls();

                    Comm.MessageboxToasted("Sales", "Voucher[" + txtPrefix.Text + txtInvAutoNo.Text + "] Saved Successfully");
                }
                catch (Exception ex)
                {
                    rs.RollbackTrans = true;
                    MessageBox.Show(ex.Message, "Barcode Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    btnFillData.Enabled = true;
                    DgvData.Cursor = Cursors.Default;
                }
                finally
                {
                    rs.Dispose();
                    rs = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Barcode Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearControls()
        {
            try
            {
                DgvData.DataSource = null;
                DgvData.Rows.Clear();

                txtFillSearch.Text = "";
                txtSearch.Text = "";

                if (cboCostCentre.Items.Count > 0)
                    cboCostCentre.SelectedIndex = 0;
                if (cboSalesStaff.Items.Count > 0)
                    cboSalesStaff.SelectedIndex = 0;
                if (cmbDisplayStyle.Items.Count > 0)
                    cmbDisplayStyle.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Barcode Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtInvAutoNo_Leave(object sender, EventArgs e)
        {
            try
            {
                Comm.ControlEnterLeave(txtInvAutoNo);
                if (iIDFromEditWindow == 0)
                {
                    DataTable dtInv = Comm.fnGetData("SELECT Invid FROM tblBarcodeManager WHERE InvNo = '" + txtInvAutoNo.Text + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                    if (dtInv.Rows.Count > 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("There is an Exisiting Bill Number in this Invoice No [" + txtInvAutoNo.Text + "]. Please enter different number.", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                        txtInvAutoNo.Clear();
                        txtInvAutoNo.Tag = 0;
                        txtInvAutoNo.Focus();

                        FetchInvNo();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Barcode Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSRate4_Click(object sender, EventArgs e)
        {

        }

        private void pnl3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblOldExpiryDate_Click(object sender, EventArgs e)
        {

        }

        private void txtOldExpiryDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbpBarcodeChanger_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate1New.Text))
            {
                MessageBox.Show("Duplicate Entry", "Error");
            }
        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvBarcodeDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvBarcodeDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnFill_Click(object sender, EventArgs e)
        {

        }

        private void txtInvoiceNumber_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtInvoiceNumber_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbVoucherType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchBarcode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchBarcode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSearchBarcode_Leave(object sender, EventArgs e)
        {

        }

        private void btnTestPrint_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveAndExport_Click(object sender, EventArgs e)
        {

        }

        private void cmbInstalledPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPrintScheme_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPrintScheme_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtLabelsPerRow_TextChanged(object sender, EventArgs e)
        {

        }

        private void trvBarcodeTags_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void txtBarcodeString_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel19_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void panel21_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkEncryptDecimals_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabBarcode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPrintScheme_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }

        private void cmbInstalledPrinters_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void txtSearchBarcode_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void tbpBarcodePrint_Click(object sender, EventArgs e)
        {

        }

        private void cmbVoucherType_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void cmbPrintScheme_KeyDown_1(object sender, KeyEventArgs e)
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

        private void txtCharWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // allows only numbers
                if (!char.IsNumber(e.KeyChar) && e.KeyChar.ToString() != "\b")
                {
                    e.Handled = true;
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

        }


        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // Verify that the pressed key isn't CTRL or any non-numeric digit
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }

                // If you want, you can allow decimal (float) numbers
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
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

        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // Verify that the pressed key isn't CTRL or any non-numeric digit
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }

                // If you want, you can allow decimal (float) numbers
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
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

        }

        private void txtItemName_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtOldPLU_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtOldPLU_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // allows only numbers
                if (!char.IsNumber(e.KeyChar) && e.KeyChar.ToString() != "\b")
                {
                    e.Handled = true;
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

        }

        private void txtCharWidth_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblInvDate_Click(object sender, EventArgs e)
        {

        }

        private void dtpInvDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblInvNo_Click(object sender, EventArgs e)
        {

        }

        private void lblHeading_Click(object sender, EventArgs e)
        {

        }

        private void cboSalesStaff_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtItemSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtItemSearch.Text != "")
                {
                    string sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                            " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                    //string sQuery = "SELECT ItemCode+ItemName As AnyWhere,ItemCode as [Item Code],ItemName as [Item Name],ItemID FROM tblItemMaster WHERE TenantID = " + Global.gblTenantID + "";
                    //new frmCompactSearch(GetFromItemSearch, sQuery, "AnyWhere|ItemCode|ItemName", txtItemSearch.Location.X + 270, txtItemSearch.Location.Y + 8, 2, 0, txtItemSearch.Text, 3, 0, "ORDER BY ItemName ASC", 0, 0, "Item Name Search ...", 0, "200,200,0", true, "frmItemMaster").ShowDialog();
                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", txtItemSearch.Location.X + 270, txtItemSearch.Location.Y + 8, 7, 0, txtItemSearch.Text, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                    frmN.Show();
                }
                else
                {
                    //ClearAll();
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

        }

        private void Initialize()
        {

            try
            {   
                txtNewBarCode.Text = txtOldBarCode.Text;
                dtpNewExpiryDate.Value = Convert.ToDateTime(txtOldExpiryDate.Text);
                txtNewPLU.Text = txtOldPLU.Text;
                txtPRateNew.Text = txtPRateOld.Text;
                txtMRPNew.Text = txtMRPOld.Text;
                txtSRate1New.Text = txtSRate1Old.Text;
                txtSRate2New.Text = txtSRate2Old.Text;
                txtSRate3New.Text = txtSRate3Old.Text;
                txtSRate4New.Text = txtSRate4Old.Text;
                txtSRate5New.Text = txtSRate5Old.Text;
                    
                if (AppSettings.IsActiveSRate2 == false)
                {
                    txtSRate2New.Enabled = false;
                }
                else
                {
                    txtSRate2New.Text = txtSRate2Old.Text;
                }
                
                if (AppSettings.IsActiveSRate3 == false)
                {
                    txtSRate3New.Enabled = false;
                }
                else
                {
                    txtSRate3New.Text = txtSRate3Old.Text;
                }
                
                if (AppSettings.IsActiveSRate4 == false)
                {
                    txtSRate4New.Enabled = false;
                }
                else
                {
                    txtSRate4New.Text = txtSRate4Old.Text;
                }
                
                if (AppSettings.IsActiveSRate5 == false)
                {
                    txtSRate5New.Enabled = false;
                }
                else
                {
                    txtSRate5New.Text = txtSRate5Old.Text;
                }
                
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Clear Controls
        private void ClearAll()
        {
            //btnRefresh.PerformClick();
            try
            {
                txtPrefix.Text = "";
                txtInvAutoNo.Text = "";
                dtpInvDate.Value = DateTime.Today;
                cboCostCentre.SelectedIndex = 0;
                cboSalesStaff.SelectedIndex = 0;

                txtItemSearch.Text = "";
                txtItemName.Text = "";
                txtOldBarCode.Text = "";
                txtOldExpiryDate.Text = "";
                txtOldPLU.Text = "0";
                txtPRateOld.Text = "0";
                txtCRateOld.Text = "0";
                txtCRateWTaxOld.Text = "0";
                txtMRPOld.Text = "0";
                txtSRate1Old.Text = "0";
                txtSRate2Old.Text = "0";
                txtSRate3Old.Text = "0";
                txtSRate4Old.Text = "0";
                txtSRate5Old.Text = "0";

                txtNewBarCode.Text = "";
                dtpNewExpiryDate.Value = DateTime.Today;
                txtNewPLU.Text = "0";
                txtPRateNew.Text = "0";
                txtMRPNew.Text = "0";
                txtSRate1New.Text = "0";
                txtSRate2New.Text = "0";
                txtSRate3New.Text = "0";
                txtSRate4New.Text = "0";
                txtSRate5New.Text = "0";
                txtItemSearch.Focus();
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
        private void FillBatchUnique()
        {
            try
            {
                DataTable dtBatchUnique = new DataTable();
                if (txtItemSearch.Tag == null) txtItemSearch.Tag = "0";
                if (txtItemSearch.Tag.ToString() == "") txtItemSearch.Tag = "0";

                dtBatchUnique = Comm.fnGetData("SELECT StockId,BatchCode,BatchUnique FROM tblStock WHERE ItemID= '" + txtItemSearch.Tag.ToString() + "' ORDER BY StockID ").Tables[0];

                var dr = dtBatchUnique.NewRow();
                dr["StockID"] = 0;
                dr["BatchUnique"] = "<All>";
                dr["BatchCode"] = "<All>";
                dtBatchUnique.Rows.InsertAt(dr, 0);
                //cboBatchUnique.DataSource = dtBatchUnique;
                //cboBatchUnique.DisplayMember = "BatchUnique";
                //cboBatchUnique.ValueMember = "StockID";
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

        private Boolean GetFromItemSearch(string sReturn)
        {
            try
            {
                DataTable dtItemPublic = new DataTable();

                if (txtItemSearch.Text=="") return false;

                DataTable dtBatch = new DataTable();
                string[] sCompSearchData = sReturn.Split('|');
                List<decimal> lstItmDisc = new List<decimal>();

                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (sCompSearchData.Length > 0)
                    {
                        if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                        {
                            GetItmMststockinfo.StockID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetItmMststockinfo.TenantID = Global.gblTenantID;

                            dtItemPublic = clsItmMst.GetItemMasterFromStock(GetItmMststockinfo);

                            if (dtItemPublic.Rows.Count > 0)
                            {
                                try
                                {
                                    this.txtItemSearch.TextChanged -= new System.EventHandler(this.txtItemSearch_TextChanged);
                                    txtItemSearch.Text = dtItemPublic.Rows[0]["ItemName"].ToString();
                                    this.txtItemSearch.TextChanged += new System.EventHandler(this.txtItemSearch_TextChanged);
                                 
                                    txtItemName.Text = dtItemPublic.Rows[0]["itemname"].ToString();
                                    txtOldBarCode.Text = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                    txtOldExpiryDate.Tag = dtItemPublic.Rows[0]["blnexpiryitem"].ToString();
                                    txtOldExpiryDate.Text = dtItemPublic.Rows[0]["ExpiryDate"].ToString();
                                    txtOldPLU.Text = dtItemPublic.Rows[0]["pluno"].ToString();
                                    txtPRateOld.Text = dtItemPublic.Rows[0]["PRate"].ToString();
                                    txtCRateOld.Text = dtItemPublic.Rows[0]["CostRateExcl"].ToString();
                                    txtCRateWTaxOld.Text = dtItemPublic.Rows[0]["CostRateInc"].ToString();
                                    txtMRPOld.Text = dtItemPublic.Rows[0]["MRP"].ToString();
                                    txtSRate1Old.Text = dtItemPublic.Rows[0]["srate1"].ToString();
                                    txtSRate2Old.Text = dtItemPublic.Rows[0]["srate2"].ToString();
                                    txtSRate3Old.Text = dtItemPublic.Rows[0]["srate3"].ToString();
                                    txtSRate4Old.Text = dtItemPublic.Rows[0]["srate4"].ToString();
                                    txtSRate5Old.Text = dtItemPublic.Rows[0]["srate5"].ToString();

                                    Initialize();
                                }
                                catch
                                {
                                    this.txtItemSearch.TextChanged += new System.EventHandler(this.txtItemSearch_TextChanged);
                                }

                                //SetValue(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                            }
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            finally
            {
            }
            
        }

     
        //Save Functionality
        private void SaveData()
        {
            if (IsValidate() == true)
            {
                /*  if (iIDFromEditWindow != 0)
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
                        }
                        Comm.MessageboxToasted("HSN Code", "BarCode modified successfully");
                }
                    */
            }
        }


        private void lblSave_Click(object sender, EventArgs e)
        {

        }

        private void txtPRateNew_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPRateNew_Leave(object sender, EventArgs e)
        {

        }
        private void TextBoxRateChecks_Leave(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt.Name == txtMRPNew.Name)
                IsValidate((TextBox)sender, true, false);
            else if (txt.Name == txtPRateNew.Name)
                IsValidate((TextBox)sender, false, true);


            IsValidate((TextBox)sender);

            //TextBox txt = new TextBox();
            //IsValidate(ref txt);
        }

        //Rate checking and Validation functionality
        private bool IsValidate(TextBox txtO = null, bool IsMRP = false, bool IsPrate = false)
        {
            bool bValidate = true;
            if (txtO != null)
            {

                if (AppSettings.IsActiveMRP == true)
                {
                    if ((IsMRP == true) && (IsPrate == false))
                    {
                        if ((Comm.ToDecimal(txtSRate1New.Text) > Comm.ToDecimal(txtMRPNew.Text)))
                        {
                            bValidate = false;
                        }

                        if (Comm.ToDecimal(txtSRate2New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                        {
                            bValidate = false;
                        }

                        if (Comm.ToDecimal(txtSRate3New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                        {
                            bValidate = false;
                        }

                        if (Comm.ToDecimal(txtSRate4New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                        {
                            bValidate = false;
                        }

                        if (Comm.ToDecimal(txtSRate5New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                        {
                            bValidate = false;
                        }
                    }
                    if (bValidate == false)
                    {
                        txtMRPNew.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if ((IsMRP == false) && (IsPrate == true))
                {
                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate1New.Text))
                    {
                        bValidate = false;
                    }

                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate2New.Text))
                    {
                        bValidate = false;
                    }

                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate3New.Text))
                    {
                        bValidate = false;
                    }

                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate4New.Text))
                    {
                        bValidate = false;
                    }

                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate5New.Text))
                    {
                        bValidate = false;
                    }

                    if (bValidate == false)
                    {
                        txtPRateNew.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate1New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate1New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate1New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate1New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate2New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate2New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate2New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate2New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate3New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate3New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate3New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate3New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate4New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate4New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate4New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate4New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate5New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate5New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }

                if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate5New.Text))
                {
                    bValidate = false;
                    if (bValidate == false)
                    {
                        txtSRate5New.BackColor = Color.FromArgb(255, 80, 80);
                    }
                }
            }
            else
            {
                try
                {
                    if (txtItemName.Text.Trim() == "")
                    {
                        bValidate = false;
                        MessageBox.Show("Please choose an Item", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtItemSearch.Focus();
                    }

                    if (AppSettings.IsActiveMRP == true)//MRP Comparison
                    {
                        if ((Comm.ToDecimal(txtSRate1New.Text) > Comm.ToDecimal(txtMRPNew.Text)))
                        {
                            bValidate = false;
                            MessageBox.Show("SRate1 should not be greater than MRP, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            txtSRate1New.Focus();
                        }

                        if (AppSettings.IsActiveSRate2 == false)
                        {
                            txtSRate2New.Enabled = false;
                        }
                        else
                        {
                            if (Comm.ToDecimal(txtSRate2New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                            {
                                bValidate = false;
                                MessageBox.Show("SRate2 should not be greater than MRP, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                txtSRate2New.Focus();
                            }
                        }

                        if (AppSettings.IsActiveSRate3 == false)
                        {
                            txtSRate3New.Enabled = false;
                        }
                        else
                        {
                            if (Comm.ToDecimal(txtSRate3New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                            {
                                bValidate = false;
                                MessageBox.Show("SRate3 should not be greater than MRP, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                txtSRate3New.Focus();
                            }
                        }

                        if (AppSettings.IsActiveSRate4 == false)
                        {
                            txtSRate4New.Enabled = false;
                        }
                        else
                        {
                            if (Comm.ToDecimal(txtSRate4New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                            {
                                bValidate = false;
                                MessageBox.Show("SRate4 should not be greater than MRP, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                txtSRate4New.Focus();
                            }
                        }

                        if (AppSettings.IsActiveSRate5 == false)
                        {
                            txtSRate5New.Enabled = false;
                        }
                        else
                        {
                            if (Comm.ToDecimal(txtSRate5New.Text) > Comm.ToDecimal(txtMRPNew.Text))
                            {
                                bValidate = false;
                                MessageBox.Show("SRate5 should not be greater than MRP, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                txtSRate5New.Focus();
                            }
                        }
                    }

                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate1New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("PRate should not be greater than SRate1, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate1New.Focus();
                    }

                    if ((Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate1New.Text)))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate should not be greater than SRate1, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate1New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate1New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate with Tax should not be greater than SRate1, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate1New.Focus();
                    }

                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate2New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("PRate should not be greater than SRate2, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate2New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate2New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate should not be greater than SRate2, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate2New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate2New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate with Tax should not be greater than SRate2, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate2New.Focus();
                    }


                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate3New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("PRate should not be greater than SRate3, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate3New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate3New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate should not be greater than SRate3, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate3New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate3New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate with Tax should not be greater than SRate3, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate3New.Focus();
                    }


                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate4New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("PRate should not be greater than SRate4, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate4New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate4New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate should not be greater than SRate4, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate4New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate4New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate with Tax should not be greater than SRate4, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate4New.Focus();
                    }


                    if (Comm.ToDecimal(txtPRateNew.Text) > Comm.ToDecimal(txtSRate5New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("PRate should not be greater than SRate5, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate5New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateOld.Text) > Comm.ToDecimal(txtSRate5New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate should not be greater than SRate5, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate5New.Focus();
                    }

                    if (Comm.ToDecimal(txtCRateWTaxOld.Text) > Comm.ToDecimal(txtSRate5New.Text))
                    {
                        bValidate = false;
                        MessageBox.Show("CRate with Tax should not be greater than SRate5, enter proper value", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        txtSRate5New.Focus();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //CHECK WHETHER ALL VALIDATIONS ARE PROPERLY CARRIED OUT. THERE'S A CHANCE SOME ISSUES ARE NOT PROPERLY VALIDATED
            return bValidate;
        }

        private void txtSRate5New_TextChanged(object sender, EventArgs e)
        {

        }
    }


        /*
if (bValidate == false)
{
return false;
}
string a = "select * from tblHSNCode where HSNCODE = '" + txtHSNCode.Text + "' and IGSTTaxPer = '" + cmbBTaxClass.Text + "'";
SqlDataAdapter da1 = new SqlDataAdapter("select * from tblHSNCode where HSNCODE='" + txtHSNCode.Text + "'  and IGSTTaxPer='" + cmbBTaxClass.Text + "'", DigiposZen.Properties.Settings.Default.ConnectionString);
DataTable dt3 = new DataTable();
da1.Fill(dt3);


if (dt3.Rows.Count > 0 && iIDFromEditWindow == 0)
{
bValidate = false;
MessageBox.Show("Duplicate Entry", "Error");

}

if (txtCompCess.Text == "" || txtCompCess.Text == null)//Nothing selected
{
txtCompCess.Text = "0";
}

if ()//no item is selected
{
bValidate = false;
MessageBox.Show("You did not select any item to modify", "Error");
}

if ()//no item is modified
{
bValidate = false;
MessageBox.Show("You did not modufy any item", "Error");
}
return bValidate;
}

*/

}
