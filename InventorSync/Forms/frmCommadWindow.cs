using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.InventorBL.Helper;

namespace DigiposZen.Forms
{
    public partial class frmCommadWindow : Form
    {

        Common Comm = new Common();

        public frmCommadWindow()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                sqlControl rs = new sqlControl();
                bool blnBeginTrans = false;

                try
                {
                    DgvData.DataSource = null;
                    DgvData.Rows.Clear();
                    DgvData.Columns.Clear();

                    if (Global.gblUserName.ToUpper() != "DIGIPOS")
                    {
                        if (txtQuery.Text.Trim().Substring(0, 7).ToUpper() != "SELECT ")
                        {
                            if (Comm.RetieveFromDBInAppSettings(Global.gblTenantID, "BlockUser_" + Global.gblUserName) == "WARNED")
                            {
                                sqlControl cn = new sqlControl();
                                cn.Execute("Update tblUserMaster Set Status=0 Where UserID=" + Global.gblUserID);
                                MessageBox.Show("Suspicious activity confirmed. User blocked.", "Advanced Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                System.Environment.Exit(0);
                            }
                            MessageBox.Show("Suspicious activity detected. Continuous usage may lead to blocking this user.");
                            Comm.SaveInAppSettings("BlockUser_" + Global.gblUserName, "WARNED");
                            return;
                        }
                    }

                    Cursor = Cursors.WaitCursor;
                    string strSql = "";
                    strSql = txtQuery.Text.ToString().Trim();
                    if (strSql != "")
                    {
                        if (txtQuery.Text.Trim().Substring(0, 7).ToUpper() == "SELECT ")
                        {
                            rs.Open(txtQuery.Text);
                            if (rs.Exception == "")
                            {
                                if (rs.eof() == false)
                                    DgvData.DataSource = rs.sqlDT;
                            }
                        }
                        else
                        {
                            rs.BeginTrans = true;
                            blnBeginTrans = true;
                            rs.Execute(txtQuery.Text);
                            if (rs.Exception != "")
                            {
                                rs.RollbackTrans = true;
                                blnBeginTrans = false;
                            }
                            else
                            {
                                DialogResult d = MessageBox.Show("This will affect " + rs.RecordCount.ToString() + " record(s). Are you sure to continue?", "Advanced Search", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (d == DialogResult.Yes)
                                {
                                    rs.CommitTrans = true;
                                    blnBeginTrans = false;
                                }
                                else
                                {
                                    rs.RollbackTrans = true;
                                    blnBeginTrans = false;
                                }
                            }
                        }
                    }

                    Cursor = Cursors.Default;
                }
                catch (Exception ex)
                {
                    if (blnBeginTrans == true)
                    {
                        rs.RollbackTrans = true;
                        blnBeginTrans = false;
                    }

                    Cursor = Cursors.Default;
                    MessageBox.Show(ex.Message, "Advanced Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void btnSearchResult_Click(object sender, EventArgs e)
        {

        }

/*        private void Search(bool blnMoveForward = true)
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
                                    if (DgvData[j, i].Visible == true)
                                    {
                                        DgvData.CurrentCell = DgvData[j, i];
                                        blnFound = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (CellValue.Contains(SearchString) == true)
                                {
                                    if (DgvData[j, i].Visible == true)
                                    {
                                        DgvData.CurrentCell = DgvData[j, i];
                                        blnFound = true;
                                        break;
                                    }
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
*/

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
    }
}
