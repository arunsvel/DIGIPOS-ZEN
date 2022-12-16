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
using DigiposZen.InventorBL.Helper;
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

            try
            {
                if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                {
                    if (iIDFromEditWindow == 0) //New
                    {
                        txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                        txtInvAutoNo.Tag = 0;
                    }
                    txtInvAutoNo.ReadOnly = true;
                    txtPrefix.ReadOnly = true;
                }
                else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                {
                    if (iIDFromEditWindow == 0) //New
                    {
                        //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum").ToString();
                        txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnSearchFwd_Click(object sender, EventArgs e)
        {
            DataGridViewCell Cell = Comm.Search(DgvData, txtSearch.Text.ToString(), true, chkMatchCase.CheckState, chkExactWordOnly.CheckState);
            if (Cell != null)
                DgvData.CurrentCell = Cell;
        }

        private void btnSearchBwd_Click(object sender, EventArgs e)
        {
            DataGridViewCell Cell = Comm.Search(DgvData, txtSearch.Text.ToString(), false, chkMatchCase.CheckState, chkExactWordOnly.CheckState);
            if (Cell != null)
                DgvData.CurrentCell = Cell;
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
                    //DgvData.Columns[2].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 1].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 3].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 2].HeaderText = "Active Status";
                    DgvData.Columns[DgvData.Columns.Count - 3].HeaderText = "Old Status";

                    //DgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                    //foreach (DataGridViewColumn col in DgvData.Columns)
                    //{
                    //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    //}

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
            sqlControl rs = new sqlControl();
            
        }
    }
}
