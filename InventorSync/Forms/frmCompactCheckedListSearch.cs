using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DigiposZen.Forms
{
    public partial class frmCompactCheckedListSearch : Form
    {
        private Boolean mblnReturnAsTextStrings = false;
        public frmCompactCheckedListSearch(dlgt_CompSearch btnSearches, string strQuery = "", string strSearchFieldName = "", int XPos = 0, int YPos = 0, int iSelectIDPos = 0, int iFieldCount = 0, string sEnterData = "", int iHideColIndexFrom = 0, int iType = 0, string strOrderBy = "", string strIDs = "", DataTable dtData = null, string sTitle = "", string sFormName = "", Boolean blnReturnAsTextStrings = false)
        {
            InitializeComponent();

            mblnReturnAsTextStrings = blnReturnAsTextStrings;

            xAxisPos = XPos;
            yAxisPos = YPos;
            sQuery = strQuery;
            sSearchFieldName = strSearchFieldName;
            btnSearchClick = btnSearches;
            iSelIDPos = iSelectIDPos;
            iFieldCnt = iFieldCount;
            dtCheckedData = dtData;
            if (sEnterData.Contains(',') == true)
                sSelectedText = sEnterData;
            else
            {
                //sEnterText = sEnterData;
                if (sEnterData.Length > 2)
                    sSelectedText = sEnterData;
            }
            sOrderBy = strOrderBy;
            iHideColumnFrom = iHideColIndexFrom;
            sIDs = strIDs;
            lblHeading.Text = sTitle;
            strFormName = sFormName;
            this.BringToFront();
            txtSearch.Focus();
            txtSearch.Select(1, 1);
        }

        #region "VARIABLES -------------------------------------------- >>"
        public delegate Boolean dlgt_CompSearch(string sSelectedIds);
        private dlgt_CompSearch btnSearchClick;
        string sQuery = "", sSearchFieldName = "", sEnterText = "", sOrderBy = "", sIDs = "", sSelectedText = "";
        int xAxisPos = 0, yAxisPos = 0, iSelIDPos = 0, iFieldCnt = 0, iHideColumnFrom;
        string sDisplayMember, sValueMember;
        int iMyDesiredIndex;
        string[] sArrIDs;
        string strFormName;
        DataTable dtCheckedData = new DataTable();
        DBConnection dbConn = new DBConnection();

        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Down)
            {
                SendKeys.Send("{Tab}");
            }
            else if (e.KeyCode == Keys.Up)
            {

            }
            else if (e.KeyCode == Keys.Space)
            {

            }
            else if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.F3)
                btnAdd.PerformClick();
        }
        private void chkListSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                WhenClickOK();
            }
        }
        private void trvCheckbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }

        private void txtSelectedItems_TextChanged(object sender, EventArgs e)
        {
            if (txtSelectedItems.Text.Trim() == ";") txtSelectedItems.Text = "";
            if (txtSelectedItems.Text.Contains(';') == true)
            {
                string stxtSelectedItems = txtSelectedItems.Text.Substring(0, txtSelectedItems.Text.Length - 1);
                if (stxtSelectedItems.Contains(',') == true)
                {
                    string[] str = stxtSelectedItems.Split(',');
                    for (int j = 0; j < str.Length; j++)
                    {
                        foreach (System.Windows.Forms.TreeNode tnNode in trvCheckbox.Nodes)
                        {
                            if (tnNode.Text.Trim().ToLower() == str[j].ToString().Replace("'", "").Trim().ToLower())
                            {
                                tnNode.Checked = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (System.Windows.Forms.TreeNode tnNode in trvCheckbox.Nodes)
                    {
                        if (tnNode.Text.Trim().ToLower() == stxtSelectedItems.ToLower())
                        {
                            tnNode.Checked = true;
                        }
                    }
                }
                this.txtSelectedItems.TextChanged -= this.txtSelectedItems_TextChanged;
                txtSelectedItems.Text = stxtSelectedItems;
                this.txtSelectedItems.TextChanged += this.txtSelectedItems_TextChanged;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (strFormName.Trim() == "frmItemCategory")
            {
                frmItemCategory frmItemcat = new frmItemCategory(0, false, txtSearch, true);
                frmItemcat.ShowDialog();
            }
            else if(strFormName.Trim() == "frmColorMaster")
            {
                frmColorMaster frmCol = new frmColorMaster(0, false, txtSearch, true);
                frmCol.ShowDialog();
            }
            else if(strFormName.Trim() == "FrmSizeMaster")
            {
                FrmSizeMaster frmsi = new FrmSizeMaster(0, false, txtSearch, true);
                frmsi.ShowDialog();
            }
            else if (strFormName.Trim() == "frmCostCentre")
            {
                frmCostCentre frmcc = new frmCostCentre(0, false, txtSearch, true);
                frmcc.ShowDialog();
            }

            if (txtSelectedItems.Text.Trim().Length>0)
            {
                txtSelectedItems.Text=txtSelectedItems.Text + "," + txtSearch.Text;
                txtSearch.Text = "";
            }

            WhenClickOK();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataTable dtSearch = new DataTable();
            dtSearch = CompactSearch(sQuery, sSearchFieldName, txtSearch.Text, sOrderBy);
            if (dtSearch != null)
            {
                PopulateTreeView(dtSearch, dtSearch.Columns[0].ColumnName, dtSearch.Columns[1].ColumnName);
                if (txtSelectedItems.Text.Contains(',') == true)
                {
                    string[] str = txtSelectedItems.Text.Split(',');
                    for (int j = 0; j < str.Length; j++)
                    {
                        foreach (System.Windows.Forms.TreeNode tnNode in trvCheckbox.Nodes)
                        {
                            if (tnNode.Text.Trim().ToLower() == str[j].ToString().Trim().ToLower())
                            {
                                tnNode.Checked = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (System.Windows.Forms.TreeNode tnNode in trvCheckbox.Nodes)
                    {
                        if (tnNode.Text.Trim().ToLower() == txtSelectedItems.Text.Trim().ToLower())
                        {
                            tnNode.Checked = true;
                        }
                    }
                }
            }
        }

        private void frmCompactCheckedListSearch_Load(object sender, EventArgs e)
        {
            DataTable dtSearch = new DataTable();
            DataTable dtCheck = new DataTable();

            this.Location = new Point(xAxisPos, yAxisPos);

            if (dtCheckedData == null)
                dtSearch = CompactSearch(sQuery, sSearchFieldName, txtSearch.Text, sOrderBy);
            else
                dtSearch = dtCheckedData;
            sDisplayMember = dtSearch.Columns[1].ColumnName;
            sValueMember = dtSearch.Columns[0].ColumnName;
            PopulateTreeView(dtSearch, sValueMember, sDisplayMember);

            txtSearch.Text = sEnterText;
            txtSelectedItems.Text = sSelectedText + ";";
        }
        private void frmCompactCheckedListSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            WhenClickOK();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region "METHODS -  -------------------------------------------- >>"
        public DataTable CompactSearch(string strQuery = "", string strSearchFieldName = "", string strSearchData = "", string strOrderBy = "")
        {
            SqlConnection sqlconn = dbConn.GetDBConnection();
            try
            {
                string sQuery = strQuery;
                if (strSearchData != "")
                {

                    if (strQuery.ToLower().Contains("where") == true)
                    {
                        sQuery = sQuery + " AND ";
                    }
                    else
                    {
                        sQuery = sQuery + " WHERE ";
                    }
                    if (strSearchFieldName != "")
                    {
                        if (strSearchData != "")
                        {
                            strSearchData = strSearchData.Replace("\r\n", "");
                            sQuery = sQuery + strSearchFieldName + " LIKE " + "'%" + strSearchData + "%'";
                        }
                        else
                        {
                            strSearchData = strSearchData.Replace("\r\n", "");
                            sQuery = sQuery + strSearchFieldName + " LIKE " + "'" + strSearchData + "'";
                        }
                    }
                    if (strOrderBy != "")
                    {
                        sQuery = sQuery + " " + strOrderBy;
                    }

                }
                else
                {
                    if (strOrderBy != "")
                    {
                        sQuery = sQuery + " " + strOrderBy;
                    }
                }
                SqlDataAdapter daSearch = new SqlDataAdapter(sQuery, sqlconn);
                DataTable dtCompSearch = new DataTable();

                daSearch.Fill(dtCompSearch);
                return dtCompSearch;
            }
            catch (Exception ex)
            {
                sqlconn.Close();
                return null;
            }
            finally
            {
                sqlconn.Close();
            }
        }
        private void PopulateTreeView(DataTable dtgetData, string sValueMember = "", string sDisplayMember = "", TreeNode parentNode = null)
        {
            trvCheckbox.Nodes.Clear();
            TreeNode childNode;
            foreach (DataRow dr in dtgetData.Rows)
            {
                if (parentNode == null)
                {
                    childNode = trvCheckbox.Nodes.Add(dr[sValueMember].ToString(), dr[sDisplayMember].ToString());
                }
                else
                {
                    parentNode.Tag = dr["ParentID"].ToString();
                    childNode = parentNode.Nodes.Add(dr[sValueMember].ToString(), dr[sDisplayMember].ToString());
                }
            }
        }
        private void WhenClickGo(string sSelIds)
        {
            if (sSelIds != "")
            {
                Boolean result = btnSearchClick(sSelIds);
                this.Close();
            }
        }
        private void WhenClickOK()
        {
            string sStrIds = "";
            int ichkCount = 0;
            foreach (System.Windows.Forms.TreeNode aNode in trvCheckbox.Nodes)
            {
                if (aNode.Checked == true)
                {
                    ichkCount = ichkCount + 1;
                }
            }
            if (ichkCount == 0)
            {
                if (strFormName.Trim() == "frmColorMaster" || strFormName.Trim() == "frmItemCategory" || strFormName.Trim() == "FrmSizeMaster" || strFormName.Trim() == "frmCostCentre")
                {
                    foreach (System.Windows.Forms.TreeNode aNode in trvCheckbox.Nodes)
                    {
                        TreeNode tn = Comm.GetNodeByText(trvCheckbox.Nodes, aNode.Text);
                        if (tn != null)
                        {
                            if(Convert.ToInt32(tn.Name) == 1)
                            {
                                aNode.Checked = true;
                                ichkCount = ichkCount + 1;
                            }
                        }
                    }
                }
            }
            if (ichkCount != 0)
            {
                txtSelectedItems.Text = Comm.GetCheckedNodesTextForChkCompact(trvCheckbox.Nodes);
                string[] schkText = txtSelectedItems.Text.Split(',');
                if (schkText.Length > 0)
                {
                    for (int i = 0; i < schkText.Length; i++)
                    {
                        TreeNode tn = Comm.GetNodeByText(trvCheckbox.Nodes, schkText[i]);
                        if (tn != null)
                        {
                            if (sStrIds == "")
                                if (mblnReturnAsTextStrings == true )
                                    sStrIds = "'" + tn.Name + "'";
                                else
                                    sStrIds = Convert.ToInt32(tn.Name).ToString();
                            else
                                if (mblnReturnAsTextStrings == true)
                                    sStrIds = sStrIds + ",'" + tn.Name + "'";
                                else    
                                    sStrIds = sStrIds + "," + Convert.ToInt32(tn.Name).ToString();
                        }
                    }
                }
            }
            WhenClickGo(sStrIds);
        }
        private string sRetIndexAsperSelectedValues(string sVal)
        {
            return "";
        }
        #endregion
    }
}
