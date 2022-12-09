using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace DigiposZen.Usercontol
{
    public partial class CompactCheckListSearch : UserControl
    {
        public delegate Boolean dlgt_CompSearch(string sSelectedIds);
        private dlgt_CompSearch btnSearchClick;

        string sQuery = "", sSearchFieldName = "", sEnterText = "", sOrderBy = "";
        int xAxisPos = 0, yAxisPos = 0, iSelIDPos = 0, iFieldCnt = 0, iHideColumnFrom;

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sStrIds = "";
            if (chkListSearch.CheckedItems.Count != 0)
            {
                for (int x = 0; x < chkListSearch.CheckedItems.Count; x++)
                {
                    sStrIds = sStrIds + (x + 1).ToString() + " = " + chkListSearch.CheckedItems[x].ToString() + ", ";
                }
            }
            WhenClickGo(sStrIds);
        }

        DBConnection dbConn = new DBConnection();

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataTable dtSearch = new DataTable();
            dtSearch = CompactSearch(sQuery, sSearchFieldName, txtSearch.Text, sOrderBy);
            ((ListBox)(chkListSearch)).DataSource = dtSearch.DefaultView;
            ((ListBox)(chkListSearch)).DisplayMember = dtSearch.Columns[1].ColumnName;
            ((ListBox)(chkListSearch)).DisplayMember = dtSearch.Columns[0].ColumnName;
        }

        private void CompactCheckListSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Hide();
        }

        

        public CompactCheckListSearch(dlgt_CompSearch btnSearches, string strQuery = "", string strSearchFieldName = "", int XPos = 0, int YPos = 0, int iSelectIDPos = 0, int iFieldCount = 0, string sEnterData = "", int iHideColIndexFrom = 0, int iType = 0, string strOrderBy = "")
        {
            InitializeComponent();

            xAxisPos = XPos;
            yAxisPos = YPos;
            sQuery = strQuery;
            sSearchFieldName = strSearchFieldName;
            btnSearchClick = btnSearches;
            iSelIDPos = iSelectIDPos;
            iFieldCnt = iFieldCount;
            sEnterText = sEnterData;
            sOrderBy = strOrderBy;
            txtSearch.Text = sEnterText;
            iHideColumnFrom = iHideColIndexFrom;
            txtSearch.Focus();
            txtSearch.Select(1, 1);
        }

        public DataTable CompactSearch(string strQuery = "", string strSearchFieldName = "", string strSearchData = "", string strOrderBy = "")
        {
            SqlConnection sqlconn = dbConn.GetDBConnection();
            try
            {
                string sQuery = strQuery;

                //if (strSearchData != "")
                //{
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

                //}
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

        private void WhenClickGo(string sSelIds)
        {
            if (txtSearch.Text == "")
            {
                MessageBox.Show("Nothing to Serarch");
            }
            else
            {
                //int cd = Convert.ToInt32(Search.SelectedValue);
                if (sSelIds != "")
                {
                    Boolean result = btnSearchClick(sSelIds);
                }
            }
        }
    }
}
