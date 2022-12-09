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
    public partial class frmDetailedSearch2 : Form
    {
        frmMDI mMDIParent = null;
        public frmDetailedSearch2(dlgt_CompSearch btnSearches, string strQuery = "", string strSearchFieldNames = "", int XPos = 0, int YPos = 0, int iSelectIDPos = 0, int iFieldCount = 0, string sEnterData = "", int iHideColIndexFrom = 0, int iType = 0, string strOrderBy = "", int iCompactSearchWidth = 0, int iShowTopCnt = 0, string sTitle = "", int iSearchColindex = -1, string sColsWidth = "", bool bShowWholeDataWhenNoSearch = false, string sFormName = "", int FirstShowRecordCnt = 0, bool bIsNumericOnly = false, object MDIParent = null, int EditColumn = 0)
        {
            InitializeComponent();

            mMDIParent = (frmMDI)MDIParent;

            xAxisPos = XPos;
            yAxisPos = YPos;
            sQuery = strQuery;
            sAllFields = strSearchFieldNames;
            //sSearchFieldName = strSearchFieldName;
            btnSearchClick = btnSearches;
            if (EditColumn == 0)
                iEditColumn = iSelIDPos;
            else
                iEditColumn = EditColumn;
            iSelIDPos = iSelectIDPos;
            iFieldCnt = iFieldCount;
            sEnterText = sEnterData;
            sOrderBy = strOrderBy;
            iHideColumnFrom = iHideColIndexFrom;
            iCompSearchWidth = iCompactSearchWidth;
            iSearchColumnIndex = iSearchColindex;
            lblHeading.Text = sTitle;
            sColumnsWidths = sColsWidth;
            bShowWholeData = bShowWholeDataWhenNoSearch;
            strFormName = sFormName;
            iShowCnt = FirstShowRecordCnt;
            bIsNumeric = bIsNumericOnly;
            Fill();
            if (bShowWholeData == true)
            {
                if (sEnterText == "")
                    txtSearch.Text = "~";
                else
                    txtSearch.Text = sEnterText;
            }
            else
                txtSearch.Text = sEnterText;

            txtSearch.SelectionStart = txtSearch.Text.Length;
            txtSearch.SelectionLength = 0;

            tableLayoutPanel1.Focus();
            Application.DoEvents();
            txtSearch.Focus();
            txtSearch.Select(1, 1);
        }
       
        #region "VARIABLES  -------------------------------------------- >>"
        Common Comm = new Common();
        string sQuery = "",     sSearchFieldName = "", sEnterText = "", sOrderBy = "", sAllFields = "", sColumnsWidths = "";
        int xAxisPos = 0, yAxisPos = 0, iSelIDPos = 0, iEditColumn = 0, iFieldCnt = 0, iHideColumnFrom, iCompSearchWidth, iSearchColumnIndex, iShowCnt;
        bool bShowWholeData,bIsNumeric;
        string strFormName;

        public delegate Boolean dlgt_CompSearch(string LstIDandText);
        private dlgt_CompSearch btnSearchClick;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        private void frmDetailedSearch2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void flowLayoutPanel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void lblHeading_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void CloseMe()
        {
            Boolean result = btnSearchClick("0|" + txtSearch.Text + "");
            this.Close();
        }
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            int iMyDesiredIndex = 0;
            if (e.KeyCode == Keys.Escape)
            {
                CloseMe();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                WhenDgvSearchEnter();
            }
            else if (e.KeyCode == Keys.Down) //Added and Commented by Anjitha on 21-Jan-2022 --->>
            {
                if (dgvSearch.Rows.Count > 0)
                {
                    if (dgvSearch.SelectedRows[0].Index < dgvSearch.Rows.Count - 1)
                        iMyDesiredIndex = dgvSearch.SelectedRows[0].Index + 1;
                    dgvSearch.ClearSelection();

                    if (iMyDesiredIndex > -1)
                        dgvSearch.Rows[iMyDesiredIndex].Selected = true;

                    dgvSearch.CurrentCell = dgvSearch.Rows[iMyDesiredIndex].Cells[1];
                    e.Handled = true;
                }
            }

            //else if (e.KeyCode == Keys.Down)
            //{
            //    if (dgvSearch.SelectedRows[0].Index < dgvSearch.Rows.Count - 1)
            //        iMyDesiredIndex = dgvSearch.SelectedRows[0].Index + 1;

            //    dgvSearch.ClearSelection();

            //    if (iMyDesiredIndex > -1)
            //        dgvSearch.Rows[iMyDesiredIndex].Selected = true;

            //    dgvSearch.CurrentCell = dgvSearch.Rows[iMyDesiredIndex].Cells[0];
            //    e.Handled = true;
            //}
            //Added and Commented by Anjitha on 21-Jan-2022 --->>
            else if (e.KeyCode == Keys.Up)
            {
                if (dgvSearch.SelectedRows[0].Index < dgvSearch.Rows.Count)
                    iMyDesiredIndex = dgvSearch.SelectedRows[0].Index - 1;

                if (iMyDesiredIndex < 0) iMyDesiredIndex = 0;
                dgvSearch.ClearSelection();

                if (iMyDesiredIndex > -1)
                    dgvSearch.Rows[iMyDesiredIndex].Selected = true;

                dgvSearch.CurrentCell = dgvSearch.Rows[iMyDesiredIndex].Cells[1];
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnAdd.PerformClick();
            }

            else if (e.KeyCode == Keys.F4)
            {
                //int sVal = Convert.ToInt32(dgvSearch.CurrentRow.Cells[iSelIDPos].FormattedValue);
                btnEdit.PerformClick();
            }
        }
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            WhenSearch();
        }

        private void frmDetailedSearch2_Load(object sender, EventArgs e)
        {
            int iColCnt = dgvSearch.Columns.Count;

            this.Location = new Point(xAxisPos, yAxisPos + 150);

            if (iHideColumnFrom != 0)
            {
                if (iColCnt > iHideColumnFrom)
                {
                    for (int i = iHideColumnFrom; i < iColCnt; i++)
                    {
                        dgvSearch.Columns[i].Width = 0;
                        dgvSearch.Columns[i].Visible = false;
                    }
                }
            }

            if (dgvSearch.Rows.Count > 0)
            {
                //dgvSearch.CurrentCell = dgvSearch[0, 0];
                dgvSearch.CurrentCell = dgvSearch[1, 0]; 
                dgvSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSearch.MultiSelect = false;
            }
            tableLayoutPanel1.Focus();
            Application.DoEvents();
            txtSearch.Focus();
            txtSearch.Select(1, 1);
            if (strFormName.Trim() == "HSNCode" || strFormName.Trim() == "Rack" || strFormName.Trim() == "Bar Code")
            {
                btnAdd.Visible = false;
                btnEdit.Visible = false;
                lblshortcut.Text = "Keyboard Shortcuts: - Esc Close";

                if (strFormName.Trim() == "Rack")
                {
                    this.Size = new Size(50, 50);
                }
            }
            else
            {
                btnAdd.Visible = true;
                btnEdit.Visible = true;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Added by Anjitha 25/03/2022 09:42 AM For HSN
            if(bIsNumeric==true)
            {
                //Allow Numeric and decimal point only
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void frmDetailedSearch2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Boolean result = btnSearchClick("0|" + txtSearch.Text + "");
                this.Close();
            }
        }
        private void frmDetailedSearch2_Shown(object sender, EventArgs e)
        {
            txtSearch.Focus();
            txtSearch.Select(1, 1);
        }
        private void cboSearchBy_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string[] strArrColumns;
            strArrColumns = sAllFields.Split('|');
            sSearchFieldName = strArrColumns[Convert.ToInt32(cboSearchBy.SelectedValue) - 1];
            if (sSearchFieldName.ToUpper() == "ANYWHERE")
            {
                sSearchFieldName = "(" + sAllFields.Replace('|', '+').Substring(9, sAllFields.Length - 9) + ")";
            }
            WhenSearch();

            txtSearch.Focus();
            txtSearch.Select(0, txtSearch.Text.Length);
        }

        private void DgvSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                WhenDgvSearchEnter();
            }
            if (e.KeyCode == Keys.Escape)
            {
                Boolean result = btnSearchClick("0|" + txtSearch.Text + "");
                this.Close();
            }
        }
        private void DgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearch.SelectedRows.Count > 0)
            {
                string sVal = dgvSearch.CurrentRow.Cells[iSelIDPos].FormattedValue.ToString();
                string sSearchText = txtSearch.Text.ToString();
                if (strFormName.Trim() == "HSNCode")
                {
                    string sTaxVal = dgvSearch.CurrentRow.Cells[1].FormattedValue.ToString();
                    WhenClickGo(sVal + "|" + sSearchText + "|" + sTaxVal);
                }
                else
                    WhenClickGo(sVal + "|" + sSearchText);
            }
        }
        private void DgvSearch_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (strFormName.Trim() == "frmItemMaster")
            {
                frmItemMaster frmM = new frmItemMaster(0, true, "S", txtSearch, true);
                frmM.ShowDialog();
                //frmM.BringToFront();

            }
            //Added by Anjitha 27/01/2022
            else if (strFormName.Trim() == "frmSupplier")
            {
                frmLedger frmLed = new frmLedger(0, true, 0, "SUPPLIER", txtSearch, true);
                frmLed.ShowDialog();
                //frmLed.BringToFront();

            }
            //Added by Anjitha 02/03/2022
            else if (strFormName.Trim() == "frmAreaMaster")
            {
                frmAreaMaster frmArea = new frmAreaMaster(0, false, txtSearch, true);
                frmArea.ShowDialog();
                //frmArea.BringToFront();

            }
            else if (strFormName.Trim() == "frmAgent")
            {
                frmAgentMaster frmAgent = new frmAgentMaster(0, false, txtSearch, true);
                frmAgent.ShowDialog();
                //frmAgent.BringToFront();

            }
            else if (strFormName.Trim() == "frmManufacture")
            {
                frmManufacturer frmManf = new frmManufacturer(0, true, txtSearch, true);
                frmManf.ShowDialog();

            }
            else if (strFormName.Trim() == "frmItemCategory")
            {
                frmItemCategory frmItemcat = new frmItemCategory(0, true, txtSearch, true);
                frmItemcat.ShowDialog();
                //frmItemcat.BringToFront();

            }
            WhenSearch();
        }
        private void btnEdit_Click(object sender, EventArgs e)//Added by Anjitha
        {
            //int sVal = Convert.ToInt32(dgvSearch.CurrentRow.Cells[iSelIDPos].FormattedValue);
            int sVal = Convert.ToInt32(dgvSearch.CurrentRow.Cells[iEditColumn].FormattedValue);
            if (strFormName.Trim() == "frmItemMaster")
            {
                frmItemMaster frmM = new frmItemMaster(sVal, true, "E", null, true);
                frmM.ShowDialog();
            }
            else if (strFormName.Trim() == "frmSupplier")
            {
                frmLedger frmLed = new frmLedger(sVal, true, 0, "SUPPLIER", txtSearch, true);
                frmLed.ShowDialog();
            }
            //Added by Anjitha 02/03/2022
            else if (strFormName.Trim() == "frmAreaMaster")
            {
                frmAreaMaster frmArea = new frmAreaMaster(sVal, true, txtSearch, true);
                frmArea.ShowDialog();
            }
            else if (strFormName.Trim() == "frmAgent")
            {
                frmAgentMaster frmAgent = new frmAgentMaster(sVal, true, txtSearch, true);
                frmAgent.ShowDialog();
            }
            else if (strFormName.Trim() == "frmManufacture")
            {
                frmManufacturer frmManf = new frmManufacturer(sVal, true, txtSearch, true);
                frmManf.ShowDialog();
            }
            else if (strFormName.Trim() == "frmItemCategory")
            {
                frmItemCategory frmItemcat = new frmItemCategory(sVal, true, txtSearch, true);
                frmItemcat.ShowDialog();
            }
            WhenSearch();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseMe();
        }
        #endregion

        #region "METHODS -  -------------------------------------------- >>"
        //Description : Fill Data in Combo
        private void Fill()
        {
            if (iSearchColumnIndex != -1)
            {
                string[] strArrColumns;
                strArrColumns = sAllFields.Split('|');
                sSearchFieldName = strArrColumns[iSearchColumnIndex];
                if (sSearchFieldName.ToUpper() == "ANYWHERE")
                {
                    sSearchFieldName = "(" + sAllFields.Replace('|', '+').Substring(9, sAllFields.Length - 9) + ")";
                }
            }
        }
        //Description : After Functionality when  Search enter in Grid 
        private void WhenDgvSearchEnter()
        {
            try
            {
                if (dgvSearch.SelectedRows.Count > 0)
                {
                    string sVal = dgvSearch.CurrentRow.Cells[iSelIDPos].FormattedValue.ToString();
                    string sSearchText = txtSearch.Text.ToString();
                    //WhenClickGo(Convert.ToInt32(sVal));
                    if (strFormName.Trim() == "HSNCode")
                    {
                        if (sSearchText != "")
                        {
                            if (sVal.ToUpper() != sSearchText.ToUpper())
                            {
                                DialogResult dlgResult = MessageBox.Show("Do you really want to create HSNCode[" + sSearchText + "] Yes ? Or No to Select the existing HSNCode [" + sVal + "].", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (dlgResult.Equals(DialogResult.Yes))
                                {
                                    WhenClickGo("NOTEXIST" + "|" + sSearchText);
                                }
                                else
                                {
                                    string sTaxVal = dgvSearch.CurrentRow.Cells[1].FormattedValue.ToString();
                                    WhenClickGo(sVal + "|" + sSearchText + "|" + sTaxVal);
                                }
                            }
                            else
                            {
                                string sTaxVal = dgvSearch.CurrentRow.Cells[1].FormattedValue.ToString();
                                WhenClickGo(sVal + "|" + sSearchText + "|" + sTaxVal);
                            }
                        }
                        else
                        {
                            string sTaxVal = dgvSearch.CurrentRow.Cells[1].FormattedValue.ToString();
                            WhenClickGo(sVal + "|" + sSearchText + "|" + sTaxVal);
                        }
                    }
                    else
                        WhenClickGo(sVal + "|" + sSearchText);
                }
                else
                {
                    string sVal = "NOTEXIST";
                    string sSearchText = txtSearch.Text;
                    WhenClickGo(sVal + "|" + sSearchText);
                }

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        //Description : Form Close functionality
        private void WhenClickGo(string LstIDandText)
        {
            if (LstIDandText != "")
            {
                Boolean result = btnSearchClick(LstIDandText);
                this.Close();
            }
        }
        //Description : Grid fill after Search
        private void WhenSearch()
        {
            string[] sArrColWidth;
            int k;
            DataTable dtSearch = new DataTable();
            int iColWidthTot = 0;

            if (txtSearch.Text == "~")
                txtSearch.Text = "";

            if (txtSearch.Text.Trim() != "")
                dtSearch = Comm.CompactSearch(sQuery, sSearchFieldName, txtSearch.Text, sOrderBy);
            else
            {
                if (bShowWholeData == false)
                    dtSearch = Comm.CompactSearch(sQuery, sSearchFieldName, txtSearch.Text, sOrderBy);
                else
                    dtSearch = Comm.CompactSearch(sQuery, "", txtSearch.Text, sOrderBy);
            }
            dgvSearch.ReadOnly = true;
            if (dtSearch != null)
            {
                if (cboSearchBy.Items.Count == 0)
                {
                    DataTable MyTable = new DataTable();
                    MyTable.Columns.Add("Id", typeof(int));
                    MyTable.Columns.Add("ColName", typeof(string));

                    if (dtSearch.Columns.Count > 0)
                    {
                        for (int i = 0; i < dtSearch.Columns.Count; i++)
                        {
                            if (i < iHideColumnFrom)
                            {
                                DataRow row = MyTable.NewRow();
                                row["Id"] = i + 1;
                                row["ColName"] = dtSearch.Columns[i].ColumnName.ToString();
                                MyTable.Rows.Add(row);
                            }
                        }
                        cboSearchBy.DataSource = MyTable;
                        cboSearchBy.DisplayMember = "ColName";
                        cboSearchBy.ValueMember = "Id";
                        cboSearchBy.SelectedIndex = iSearchColumnIndex;
                    }
                }

                //for (int i = dtSearch.Columns.Count - 1; i >= 0; i--)
                //{
                //    DataColumn dc = dtSearch.Columns[i];
                //    if (dtSearch.Columns[i].ColumnName.ToUpper().Contains("ANYWHERE"))
                //    {
                //        dtSearch.Columns.Remove(dc);
                //    }
                //}

                dgvSearch.DataSource = dtSearch.DefaultView;

                if (dgvSearch.Rows.Count > 0)
                {
                    dgvSearch.CurrentCell = dgvSearch[1, 0];

                    for (int i = dgvSearch.Columns.Count - 1; i >= 0; i--)
                    {
                        if (dgvSearch.Columns[i].HeaderText.ToUpper().Contains("ANYWHERE"))
                        {

                            dgvSearch.Columns[i].Visible = false;
                        }
                    }
                }

                if (sColumnsWidths != "")
                {
                    sArrColWidth = sColumnsWidths.Split(',');
                    for (k = 0; k < sArrColWidth.Length; k++)
                    {
                        dgvSearch.Columns[k].Width = Convert.ToInt32(sArrColWidth[k].ToString());
                        iColWidthTot = iColWidthTot + Convert.ToInt32(sArrColWidth[k].ToString());
                    }
                    if (sArrColWidth.Length == 2)
                    {
                        if (iColWidthTot <= 315)
                            dgvSearch.Columns[0].Width = 300;
                    }
                }
                else
                {
                    if (dgvSearch.Columns.Count > 0)
                    {
                        dgvSearch.Columns[0].Width = 200;
                    }
                }
                //this.Width = iColWidthTot + 20;

                //if (dtSearch.Rows.Count > 30)
                //    this.Height = this.Height + 300;
                //else if (dtSearch.Rows.Count > 10)
                //    this.Height = this.Height + 150;
                //else
                //    this.Height = 350;

                if (iShowCnt > 0)
                {
                    this.Height = iShowCnt + 150;
                }
                else
                {
                    if (dtSearch.Rows.Count > 30)
                        this.Height = this.Height + 300;
                    else if (dtSearch.Rows.Count > 10)
                        this.Height = this.Height + 150;
                    else
                        this.Height = 350;
                }

                #region "Theming ------------------------------------- >>"
                //dgvSearch.BorderStyle = BorderStyle.None;
                //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
                //dgvSearch.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                //dgvSearch.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
                //dgvSearch.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
                //dgvSearch.BackgroundColor = Color.White;

                dgvSearch.EnableHeadersVisualStyles = false;
                dgvSearch.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgvSearch.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(65, 85, 104);
                dgvSearch.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                #endregion

                if (dgvSearch.Rows.Count > 0)
                {
                    dgvSearch.DefaultCellStyle.SelectionBackColor = Color.Yellow;
                    dgvSearch.DefaultCellStyle.SelectionForeColor = Color.Black;

                    /*//Commented by Anjitha*/
                    //dgvSearch.Rows[0].Selected = true;

                    //dgvSearch.Focus();

                    //dgvSearch.CurrentCell = dgvSearch[0, 0];
                    dgvSearch.CurrentCell = dgvSearch[1, 0]; 
                    dgvSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvSearch.MultiSelect = false;

                    //Added by Anjitha
                    dgvSearch.Rows[0].Selected = true;
                }
                Application.DoEvents();

                txtSearch.Focus();

            }
        }
        #endregion
    }
}
