using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.Info;
using DigiposZen.InventorBL.Helper;
using DigiposZen.InventorBL.Master;
using DigiposZen.Forms;
using System.Runtime.InteropServices;

namespace DigiposZen
{
    public partial class frmItemAnalysis : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description:  Item Analysis Report          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 24/02/2022 6:21 PM
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

        public frmItemAnalysis()
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
                lblRefresh.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblRefresh.ForeColor = Color.Black;

                lblRefresh.Image = global::DigiposZen.Properties.Resources.refresh_removebg1;
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

            txtSearchItem.Focus();
            txtSearchItem.Select();
            FillVoucherType();
            FillBatchUnique();
            FillCostCentre();
            ClearAll();
            this.BackColor = Global.gblFormBorderColor;
            Cursor.Current = Cursors.Default;
        }

        #region "VARIABLES --------------------------------------------- >>"
        bool dragging = false;
        int xOffset = 0, yOffset = 0;

        //Info
        UspGetCostCentreInfo GetCctinfo = new UspGetCostCentreInfo();
        UspGetItemMasterInfo GetItmMstInfo = new UspGetItemMasterInfo();
        UspGetStockHistoryInfo GetStockHisIfo = new UspGetStockHistoryInfo();

        clsCostCentre clscct = new clsCostCentre();
        clsItemMaster clsItmMst = new clsItemMaster();
        clsItemAnalysis clsItmAnalys = new clsItemAnalysis();
        Common Comm = new Common();

        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        private void frmItemAnalysis_Load(object sender, EventArgs e)
        {
            dtpFromDate.MinDate = AppSettings.FinYearStart;
            dtpFromDate.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            dtpToDate.MinDate = AppSettings.FinYearStart; 
            dtpToDate.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            rdoStockHistory.Checked = true;
        }
        private void frmItemAnalysis_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)
                {
                    //DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    //if (dlgResult.Equals(DialogResult.Yes))
                        this.Close();
                }
                else if (e.KeyCode == Keys.F5)//Refresh
                {
                    try
                    {
                        btnRefresh_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                    }
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
        private void tlpHeading_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpHeading_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }
        private void tlpHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            //DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //if (dlgResult.Equals(DialogResult.Yes))
                this.Close();
        }
        private void txtSearchItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtSearchItem.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboVoucherType.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void cboVoucherType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtSearchItem.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboBatchUnique.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void cboBatchUnique_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboVoucherType.Focus();
                cboVoucherType.Select();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboCostCentre.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboBatchUnique.Focus();
                cboBatchUnique.Select();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                dtpFromDate.Focus();
            }
        }
        private void dtpFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboCostCentre.Focus();
                cboCostCentre.Select();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                dtpToDate.Focus();
            }
        }
        private void dtpToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                dtpFromDate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                btnRefresh.PerformClick();
                dgvStockHistory.Focus();
            }
        }
        private void txtSearchItem_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchItem.Text != "")
            {
                string sQuery = "SELECT ItemCode+ItemName As AnyWhere,ItemCode as [Item Code],ItemName as [Item Name],ItemID FROM tblItemMaster WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactSearch(GetFromItemSearch, sQuery, "AnyWhere|ItemCode|ItemName", txtSearchItem.Location.X + 270, txtSearchItem.Location.Y + 8, 2, 0, txtSearchItem.Text, 3, 0, "ORDER BY ItemName ASC", 0, 0, "Item Name Search ...", 0, "200,200,0", true, "frmLedger").ShowDialog();
                FillBatchUnique();
                SendKeys.Send("{Tab}");
            }
            else
            {
                ClearAll();
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                decimal dTotIn = 0, dTotOut = 0;
                if (rdoStockHistory.Checked == true)
                {
                    if (IsValidate() == true)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        DataTable dtStHistory = new DataTable();
                        GetStockHisIfo.ItemID = Convert.ToDecimal(lblItemID.Text);
                        GetStockHisIfo.VchTypID = Convert.ToDecimal(cboVoucherType.SelectedValue);
                        GetStockHisIfo.BatchUnique = Convert.ToDecimal(cboBatchUnique.SelectedValue);
                        GetStockHisIfo.CostCentreID = Convert.ToDecimal(cboCostCentre.SelectedValue);
                        GetStockHisIfo.FromDate = Convert.ToDateTime(dtpFromDate.Text).ToString("dd-MMM-yyyy");
                        GetStockHisIfo.ToDate = Convert.ToDateTime(dtpToDate.Text).ToString("dd-MMM-yyyy");
                        GetStockHisIfo.TenantID = Convert.ToDecimal(Global.gblTenantID);

                        //Show Grid Data
                        dtStHistory = clsItmAnalys.GetStockHistory(GetStockHisIfo);
                        if (dtStHistory.Rows.Count <= 0)
                        {
                            MessageBox.Show("No Stock History related your Search.Please Check Search Data", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        dgvStockHistory.DataSource = dtStHistory;
                        DataGridAlignment();

                        for (int j = 0; j < dgvStockHistory.Rows.Count; j++)
                        {
                            //dgvStockHistory.Rows[j].Cells[1].Value = Comm.chkChangeValuetoZero(dgvStockHistory.Rows[j].Cells[1].Value.ToString());
                            //dgvStockHistory.Rows[j].Cells[2].Value = Comm.chkChangeValuetoZero(dgvStockHistory.Rows[j].Cells[2].Value.ToString());
                            //dgvStockHistory.Rows[j].Cells[3].Value = Comm.chkChangeValuetoZero(dgvStockHistory.Rows[j].Cells[3].Value.ToString());
                            dTotIn = dTotIn + Convert.ToDecimal(dgvStockHistory.Rows[j].Cells[4].Value);
                            dTotOut = dTotOut + Convert.ToDecimal(dgvStockHistory.Rows[j].Cells[5].Value);
                        }

                        lblQtyInAmt.Text = Comm.chkChangeValuetoZero(dTotIn.ToString(AppSettings.QtyDecimalFormat));
                        lblQtyOutAmt.Text = Comm.chkChangeValuetoZero(dTotOut.ToString(AppSettings.QtyDecimalFormat));
                        lblQOHAmt.Text = Comm.chkChangeValuetoZero((dTotIn - dTotOut).ToString(AppSettings.QtyDecimalFormat));

                        //Show Bottom Part Data(Total QTYIn,Total QtyOut,QOH)
                        //DataTable dtTotal = clsItmAnalys.GetStockHistoryTotal(GetStockHisIfo);
                        //if (dtTotal.Rows.Count > 0)
                        //{
                        //    //lblQtyInAmt.Text = dtTotal.Rows[0]["TotalQTYIn"].ToString();
                        //    lblQtyInAmt.Text = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(dtTotal.Rows[0]["TotalQTYIn"].ToString()), false, ""));
                        //    lblQtyOutAmt.Text = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(dtTotal.Rows[0]["TotalQTYOut"].ToString()), false, ""));
                        //    lblQOHAmt.Text = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(dtTotal.Rows[0]["BalanceQty"].ToString()), false, ""));
                        //}
                        Cursor.Current = Cursors.Default;
                    }
                }
                else if (rdoStockReport.Checked == true)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    DataTable dtStReport = new DataTable();
                    
                    GetStockHisIfo.ToDate = Convert.ToDateTime(dtpToDate.Text).ToString("dd-MMM-yyyy");
                    GetStockHisIfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                    dtStReport = clsItmAnalys.GetStockReport(GetStockHisIfo);
                    if (dtStReport.Rows.Count <= 0)
                    {
                        MessageBox.Show("No Stock related your Search.Please Check Search Data", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    dgvStockHistory.DataSource = dtStReport;
                    DataGridAlignment();

                    for (int j = 0; j < dgvStockHistory.Rows.Count; j++)
                    {
                        //dgvStockHistory.Rows[j].Cells[1].Value = Comm.chkChangeValuetoZero(dgvStockHistory.Rows[j].Cells[1].Value.ToString());
                        //dgvStockHistory.Rows[j].Cells[2].Value = Comm.chkChangeValuetoZero(dgvStockHistory.Rows[j].Cells[2].Value.ToString());
                        //dgvStockHistory.Rows[j].Cells[3].Value = Comm.chkChangeValuetoZero(dgvStockHistory.Rows[j].Cells[3].Value.ToString());
                        dTotIn = dTotIn + Convert.ToDecimal(dgvStockHistory.Rows[j].Cells[1].Value);
                        dTotOut = dTotOut + Convert.ToDecimal(dgvStockHistory.Rows[j].Cells[2].Value);
                    }

                    //string sQuery = "SELECT Sum(isnull(QtyIn, 0)) as TotalQTYIn ,sum(isnull(QtyOut, 0)) as TotalQTYOut ,(Sum(isnull(QtyIn, 0)) - sum(isnull(QtyOut, 0))) as BalanceQty FROM tblStockHistory SH LEFT JOIN  tblstock S ON SH.BatchUnique = S.BatchUnique LEFT JOIN  VWitemAnalysis VwIA  ON SH.RefId = VwIA.Invid LEFT JOIN  tblItemMaster IM    ON SH.ItemID = IM.ItemID LEFT JOIN  tblUnit U  ON IM.UNITID = U.UnitID WHERE  SH.TenantID = " +Global.gblTenantID + " AND convert(datetime, VchDate, 106) <= '" + dtpToDate.Text + "' AND VwIA.VchType IS NOT NULL";
                    //DataTable dtTotal = Comm.fnGetData(sQuery).Tables[0];
                    //if (dtTotal.Rows.Count > 0)
                    //{
                    //    lblQtyInAmt.Text = FormatValue(Convert.ToDouble(dtTotal.Rows[0]["TotalQTYIn"].ToString()), false, "");
                    //    lblQtyOutAmt.Text = FormatValue(Convert.ToDouble(dtTotal.Rows[0]["TotalQTYOut"].ToString()), false, "");
                    //    lblQOHAmt.Text = FormatValue(Convert.ToDouble(dtTotal.Rows[0]["BalanceQty"].ToString()), false, "");
                    //}

                    lblQtyInAmt.Text = Comm.chkChangeValuetoZero(dTotIn.ToString(AppSettings.QtyDecimalFormat));
                    lblQtyOutAmt.Text = Comm.chkChangeValuetoZero(dTotOut.ToString(AppSettings.QtyDecimalFormat));
                    lblQOHAmt.Text = Comm.chkChangeValuetoZero((dTotIn - dTotOut).ToString(AppSettings.QtyDecimalFormat));

                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Load  Data from Edit" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            bool bResult = true;
            if (txtSearchItem.Text == "")
            {
                MessageBox.Show("Please Enter the Search Item Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearchItem.Select();
                bResult = false;
            }
            return bResult;
        }
        //Description : Grid Alignment
        private void DataGridAlignment()
        {
            dgvStockHistory.ReadOnly = true;
            dgvStockHistory.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F);
            dgvStockHistory.DefaultCellStyle.SelectionBackColor = Color.LightYellow;
            dgvStockHistory.DefaultCellStyle.SelectionForeColor = Color.Black;

            if (rdoStockHistory.Checked == true)
            {
                dgvStockHistory.Columns["Qty In"].DefaultCellStyle.Format = AppSettings.QtyDecimalFormat;
                dgvStockHistory.Columns["Qty Out"].DefaultCellStyle.Format = AppSettings.QtyDecimalFormat;
                dgvStockHistory.Columns["P.Rate"].DefaultCellStyle.Format = AppSettings.CurrDecimalFormat;
                dgvStockHistory.Columns["S.Rate"].DefaultCellStyle.Format = AppSettings.CurrDecimalFormat;

                //Width
                dgvStockHistory.Columns["Voucher Type"].Width = 150;
                dgvStockHistory.Columns["Invoice No"].Width = 100;
                dgvStockHistory.Columns["Voucher Date"].Width = 120;
                dgvStockHistory.Columns["Batch"].Width = 350;
                dgvStockHistory.Columns["Qty In"].Width = 80;
                dgvStockHistory.Columns["Qty Out"].Width = 90;
                dgvStockHistory.Columns["Unit"].Width = 80;
                dgvStockHistory.Columns["P.Rate"].Width = 90;
                dgvStockHistory.Columns["S.Rate"].Width = 90;

                //Alignment
                dgvStockHistory.Columns["Invoice No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvStockHistory.Columns["Voucher Date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvStockHistory.Columns["Unit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvStockHistory.Columns["Qty In"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockHistory.Columns["Qty Out"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockHistory.Columns["P.Rate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockHistory.Columns["S.Rate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                dgvStockHistory.Columns["ItemID"].Visible = false;

                //Width
                dgvStockHistory.Columns["Item Name"].Width = 540;
                dgvStockHistory.Columns["Qty In"].Width = 100;
                dgvStockHistory.Columns["Qty Out"].Width = 100;
                dgvStockHistory.Columns["QOH"].Width = 100;
                dgvStockHistory.Columns["ItemID"].Width = 0;

                //Alignment
                dgvStockHistory.Columns["Item Name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgvStockHistory.Columns["Qty In"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockHistory.Columns["Qty Out"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockHistory.Columns["QOH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockHistory.Columns["ItemID"].Width = 0;
            }

            //dgvStockHistory.Columns["Qty In"].DefaultCellStyle.Format = AppSettings.QtyDecimalFormat;
            //dgvStockHistory.Columns["Qty Out"].DefaultCellStyle.Format = AppSettings.QtyDecimalFormat;
            //dgvStockHistory.Columns["QOH"].DefaultCellStyle.Format = AppSettings.QtyDecimalFormat;

            ////Width
            //dgvStockHistory.Columns["Voucher Type"].Width = 150;
            //dgvStockHistory.Columns["Invoice No"].Width = 100;
            //dgvStockHistory.Columns["Voucher Date"].Width = 120;
            //dgvStockHistory.Columns["Batch"].Width = 350;
            //dgvStockHistory.Columns["Qty In"].Width = 80;
            //dgvStockHistory.Columns["Qty Out"].Width = 90;
            //dgvStockHistory.Columns["Unit"].Width = 80;
            //dgvStockHistory.Columns["P.Rate"].Width = 90;
            //dgvStockHistory.Columns["S.Rate"].Width = 90;

            ////Alignment
            //dgvStockHistory.Columns["Invoice No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dgvStockHistory.Columns["Voucher Date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dgvStockHistory.Columns["Unit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dgvStockHistory.Columns["Qty In"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvStockHistory.Columns["Qty Out"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvStockHistory.Columns["P.Rate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvStockHistory.Columns["S.Rate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }
        //Description : Load VoucherType to combobox
        private void FillVoucherType()
        {
            DataTable dtVoucherTyp = new DataTable();
            dtVoucherTyp = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID From tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) ORDER BY SortOrder Asc").Tables[0];

            var dr = dtVoucherTyp.NewRow();
            dr["VchTypeID"] = 0;
            dr["VchType"] = "<All>";
            dtVoucherTyp.Rows.InsertAt(dr, 0);
            cboVoucherType.DataSource = dtVoucherTyp;
            cboVoucherType.DisplayMember = "VchType";
            cboVoucherType.ValueMember = "VchTypeID";
        }
        //Description : Load BatchUnique to combobox
        private void FillBatchUnique()
        {
            DataTable dtBatchUnique = new DataTable();
            if (lblItemID.Text == "") lblItemID.Text = "0";

                dtBatchUnique = Comm.fnGetData("SELECT StockId,BatchCode,BatchUnique FROM tblStock WHERE ItemID= '" + lblItemID.Text + "' ORDER BY StockID ").Tables[0];
            
            var dr = dtBatchUnique.NewRow();
            dr["StockID"] = 0;
            dr["BatchUnique"] = "<All>";
            dtBatchUnique.Rows.InsertAt(dr, 0);
            cboBatchUnique.DataSource = dtBatchUnique;
            cboBatchUnique.DisplayMember = "BatchUnique";
            cboBatchUnique.ValueMember = "StockID";
        }
        //Description : Load Cost Centre to combobox
        private void FillCostCentre(int iSelID = 0)
        {
            DataTable dtCostCentre = new DataTable();
            dtCostCentre = Comm.fnGetData("select CCID,CCName from tblCostCentre ORDER BY CCID").Tables[0];

            var dr = dtCostCentre.NewRow();
            dr["CCID"] = 0;
            dr["CCName"] = "<All>";
            dtCostCentre.Rows.InsertAt(dr, 0);
            cboCostCentre.DataSource = dtCostCentre;
            cboCostCentre.DisplayMember = "CCName";
            cboCostCentre.ValueMember = "CCID";
        }
        //Description : Get From Item Search
        private Boolean GetFromItemSearch(string sReturn)
        {
            string[] sCompSearchData = sReturn.Split('|');
            DataTable dtItemMaster = new DataTable();

            if (sCompSearchData.Length > 0)
            {
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        GetItmMstInfo.ItemID = Convert.ToInt32(sCompSearchData[0].ToString());
                        GetItmMstInfo.TenantID = Global.gblTenantID;
                        dtItemMaster = clsItmMst.GetItemMaster(GetItmMstInfo);
                        if (dtItemMaster.Rows.Count > 0)
                        {
                            this.txtSearchItem.TextChanged -= this.txtSearchItem_TextChanged;
                            txtSearchItem.Text = dtItemMaster.Rows[0]["ItemCode"].ToString();
                            this.txtSearchItem.TextChanged += this.txtSearchItem_TextChanged;
                            lblItemID.Text = dtItemMaster.Rows[0]["ItemID"].ToString();
                        }
                        return true;
                    }
                    else
                    {
                        this.txtSearchItem.TextChanged -= this.txtSearchItem_TextChanged;
                        txtSearchItem.Text = sCompSearchData[1].ToString();
                        this.txtSearchItem.TextChanged += this.txtSearchItem_TextChanged;
                        return true;
                    }
                }
            }
            else
                return false;
        }
        //Description : Clear Controls
        private void ClearAll()
        {
            txtSearchItem.Text = "";
            cboVoucherType.SelectedIndex = 0;
            cboBatchUnique.SelectedIndex = 0;
            cboCostCentre.SelectedIndex = 0;
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;
            lblQOHAmt.Text = "000000";
            lblQtyOutAmt.Text= "000000";
            lblQtyInAmt.Text = "000000";
            txtSearchItem.Focus();
            lblItemID.Text = "0";
            //btnRefresh.PerformClick();
        }

        private void rdoStockReport_Click(object sender, EventArgs e)
        {
            lblQtyInAmt.Text = "0";
            lblQtyOutAmt.Text = "0";
            lblQOHAmt.Text = "0";

            lblItemName.Visible = false;
            lblVoucherType.Visible = false;
            lblBatchUnique.Visible = false;
            lblCostCentre.Visible = false;
            lblFromDate.Visible = false;
            txtSearchItem.Visible = false;
            cboVoucherType.Visible = false;
            cboBatchUnique.Visible = false;
            cboCostCentre.Visible = false;
            dtpFromDate.Visible = false;
            dtpToDate.Focus();
            dtpToDate.Select();
            dgvStockHistory.DataSource = null;
            lblRptHeading.Text = "Item Stock Report";
            //ClearAll();
        }

        private void rdoStockHistory_Click(object sender, EventArgs e)
        {
            lblQtyInAmt.Text = "0";
            lblQtyOutAmt.Text = "0";
            lblQOHAmt.Text = "0";

            lblItemName.Visible = true;
            lblVoucherType.Visible = true;
            lblBatchUnique.Visible = true;
            lblCostCentre.Visible = true;
            lblFromDate.Visible = true;
            txtSearchItem.Visible = true;
            cboVoucherType.Visible = true;
            cboBatchUnique.Visible = true;
            cboCostCentre.Visible = true;
            dtpFromDate.Visible = true;
            dgvStockHistory.DataSource = null;
            lblRptHeading.Text = "Item Analysis Details";
            //ClearAll();
        }

        public string FormatValue(double myValue, bool blnIsCurrency = true, string sMyFormat = "")
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
        #endregion
    }
}
