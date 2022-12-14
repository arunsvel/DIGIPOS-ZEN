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

        public frmBarcodeManager(object MDIParent = null)
        {
            InitializeComponent();

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            this.SetBounds(5, 0, l, t);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Search(bool blnMoveForward = true)
        {
            try
            {
                int rowstartindex = 0;
                int colstartindex = 0;

                bool blnMatchCase = false;
                bool blnExactWord = false;

                if (chkMatchCase.CheckState == CheckState.Checked)
                    blnMatchCase = true;
                if (chkExactWordOnly.CheckState == CheckState.Checked)
                    blnExactWord = true;

                string SearchString = txtSearch.Text.ToString();
                string CellValue = "";
                
                if (blnMatchCase == false) //If search and to be searhed in same case it will be searched
                    SearchString = SearchString.ToUpper();

                if (DgvData != null)
                {
                    if (DgvData.RowCount > 0)
                    {
                        if (DgvData.CurrentCell != null)
                        {
                            if (DgvData.CurrentCell.RowIndex >= 0)
                            {
                                rowstartindex = DgvData.CurrentCell.RowIndex;
                            }
                            if (DgvData.CurrentCell.ColumnIndex >= 0)
                            {
                                colstartindex = DgvData.CurrentCell.ColumnIndex;
                            }
                        }
                    }
                }

                bool blnFound = false;

                if (blnMoveForward == true)
                {
                    for (int i = rowstartindex; i < DgvData.Rows.Count - 1; i++)
                    {
                        if (blnFound == true) break;
                        for (int j = colstartindex; j < DgvData.Columns.Count - 1; j++)
                        {
                            //If search and CellValue in same case it will be matched
                            //string comparison is case sensitive
                            CellValue = DgvData[j, i].Value.ToString();
                            if (blnMatchCase == false)
                                CellValue = DgvData[j, i].Value.ToString().ToUpper();

                            if (blnExactWord == true)
                            {
                                if (CellValue == SearchString)
                                {
                                    DgvData.CurrentCell = DgvData[j, i];
                                    blnFound = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (CellValue.Contains(SearchString) == true)
                                {
                                    DgvData.CurrentCell = DgvData[j, i];
                                    blnFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = rowstartindex; i >= 0; i--)
                    {
                        if (blnFound == true) break;
                        for (int j = colstartindex; j >= 0; j--)
                        {
                            //If search and CellValue in same case it will be matched
                            //string comparison is case sensitive
                            CellValue = DgvData[j, i].Value.ToString();
                            if (blnMatchCase == false)
                                CellValue = DgvData[j, i].Value.ToString().ToUpper();

                            if (blnExactWord == true)
                            {
                                if (CellValue == SearchString)
                                {
                                    DgvData.CurrentCell = DgvData[j, i];
                                    blnFound = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (CellValue.Contains(SearchString) == true)
                                {
                                    DgvData.CurrentCell = DgvData[j, i];
                                    blnFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (blnFound == false)
                {
                    MessageBox.Show("Finished search. No more occurance found for search text.", "Advanced Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            { }
        }

        private void btnSearchFwd_Click(object sender, EventArgs e)
        {
            Search(true);
        }

        private void btnSearchBwd_Click(object sender, EventArgs e)
        {
            Search(false);
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
                if (txtSearch.Text != "")
                    whereSQL = " and  dbo.tblItemMaster.ItemCode + dbo.tblItemMaster.ItemName + dbo.tblStock.BatchUnique like '%" + txtSearch.Text + "%' ";
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
                            QuerySQL = "and  ItemStockID in(SELECT  [ItemStockID]   FROM [tblStock]   where BatchUnique  not in(Select BatchUnique from tblItemHistory) ";
                            break;
                        }

                    case "NEGATIVE /ZERO QTY BATCHES":
                        {
                            QuerySQL = " and isnull(tblStock.qty,0)<=0 ";
                            break;
                        }

                    case "ACTIVE BATCHES":
                        {
                            QuerySQL = " and isnull(tblStock.ActiveStatus,0)=1 ";
                            break;
                        }

                    case "DEACTIVE BATCHES":
                        {
                            QuerySQL = " and isnull(tblStock.ActiveStatus,0)=0 ";
                            break;
                        }
                }

                string SQL = @" SELECT   dbo.tblItemMaster.ItemID, dbo.tblStock.StockID,  dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName, dbo.tblStock.BatchUnique,tblStock.qoh AS Qty, 
                    CONVERT(DECIMAL(20," + AppSettings.QtyDecimals + "), dbo.tblStock.Prate ) as Prate, CONVERT(DECIMAL(20," + AppSettings.QtyDecimals + "), dbo.tblStock.CostRateExcl ) as Crate,CONVERT(DECIMAL(20," + AppSettings.QtyDecimals + "), dbo.tblStock.MRP ) as MRP, isnull(tblStock.StockActiveStatus ,1) as ActiveStatus    FROM    dbo.tblItemMaster INNER JOIN    dbo.tblStock ON dbo.tblItemMaster.ItemID = dbo.tblStock.ItemID  WHERE  (dbo.tblItemMaster.ActiveStatus = 1) " + whereSQL + QuerySQL + "   ORDER BY dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName ";
                DgvData.Rows.Clear();
                DgvData.Columns.Clear();
                DgvData.DataSource = Comm.fnGetData(SQL).Tables[0];

                

                //loadcontrol(DgvData, SQL);

                if (DgvData.Rows.Count > 0)
                {
                    DgvData.Columns.Insert(DgvData.Columns.Count - 1, new DataGridViewCheckBoxColumn());

                    DgvData.Columns[1].Visible = false;
                    DgvData.Columns[2].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 1].Visible = false;
                    DgvData.Columns[DgvData.Columns.Count - 2].HeaderText = "Active Status";

                    DgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

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
    }
}
