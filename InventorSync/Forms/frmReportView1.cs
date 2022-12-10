using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;


using DigiposZen.InventorBL.Helper;
using Microsoft.VisualBasic;

namespace DigiposZen.Forms
{
    public partial class frmReportView1 : Form
    {
        string mSql = "";

        Common Comm = new Common();

        public frmReportView1(string sFormName = "", string vchtype = "", string cost = "", string Ledger = "", string from = "", string to = "", object MDIParent = null, string sql1 = "", string amt = "", string ids = "", string area = "", string supplier = "", string product = "", string mnf = "",string item ="" )
        {
            InitializeComponent();

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            int l = form.ClientSize.Width - 20; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            int t = form.ClientSize.Height - 100; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            this.SetBounds(5, 0, l, t);

            dataGridView1.Width = this.Width - 10;
            dataGridView1.Height = this.Height - 20;


            strFormName = sFormName;
            strVchtype = vchtype;
            strFrom = from;
            strTo = to;
            strcost = cost;
            strLedger = Ledger;
            stramt = amt;
            strids = ids;
            strArea = area;
            mSql = sql1;
            strsupplier = supplier;
            strproduct = product;
            strmnf = mnf;
            stritem = item;
        }
        string strFormName;
        string strVchtype;
        string strcost;
        string strFrom;
        string strTo;
        string strLedger;
        string stramt;
        string strids;
        string strArea;
        string strsupplier;
        string strproduct;
        string strmnf;
        string stritem;
        private void frmReportView1_Load(object sender, EventArgs e)
        {


            try
            {
                dataGridView1.ReadOnly = true;

                label1.Text = strFormName + " Report";
                if (strFormName == "Sales Daybook" || strFormName == "Sales Detail Daybook" || strFormName == "Sales Hsncode Wise" || strFormName == "Sales Item Wise" || strFormName == "Sales Tax split" || strFormName == "Purchase Daybook" || strFormName == "Purchase Detail Daybook" || strFormName == "Purchase Hsncode Wise" || strFormName == "Purchase Item Wise" || strFormName == "Purchase Tax split" || strFormName == "Sales Discount" || strFormName == "Purchase Return Daybook" || strFormName == "Purchase Return Daybook Details" || strFormName == "Purchase Return Hsncode Wise" || strFormName == "Purchase Return Tax split" || strFormName == "Purchase Return Item Wise" || strFormName == "Sales Return Daybook" || strFormName == "Sales Return Daybook Details" || strFormName == "Sales Return Hsncode Wise" || strFormName == "Sales Return Item Wise" || strFormName == "Sales Return Tax split" || strFormName == "Sales Return Discount" || strFormName == "Delivery Note Daybook" || strFormName == "Delivery Note Detail Daybook" || strFormName == "Delivery Note Item Wise" || strFormName == "Receipt Note Daybook" || strFormName == "Receipt Note Detail Daybook" || strFormName == "Receipt Note Item Wise")
                {
                    string a = "";

                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Cost Center :" + strcost + ",Voucher Type:" + strVchtype + "";
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwpurchase ORDER BY [Invoice No],[Invoice Date]", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwpurchase");
                    dataGridView1.DataSource = ds.Tables["vwpurchase"].DefaultView;


                    label1.Text = strFormName + " Report";
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        string columnName = dataGridView1.Columns[i].Name;
                        if (columnName == "Tax Amount " || columnName == "Gross Amount" || columnName == "Item Discount" || columnName == "Qty" || columnName == "ItemDiscount Total" || columnName == "Non Taxable" || columnName == "Taxable" || columnName == "CGSTTotal" || columnName == "SGSTTotal" || columnName == "IGSTTotal" || columnName == "FloodCess Total" || columnName == "CashDiscount" || columnName == "OtherExpense" || columnName == "Net Amount" || columnName == "NetAmount" || columnName == "RoundOff" || columnName == "Bill Amount")
                        {
                            a = a + "cast(sum([" + columnName + "])as numeric(36,2)) as N" + i + ",";

                        }
                        else
                        {
                            a = a + " Null as N" + i + ",";
                        }

                    }
                    string b = a.Remove(a.Length - 1, 1);

                    string sql = "select * from vwpurchase  union all SELECT " + b + "  FROM vwpurchase ORDER BY AutoNum";
                    SqlDataAdapter da1 = new SqlDataAdapter(sql, DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1, "vwpurchase");
                    dataGridView1.DataSource = ds1.Tables["vwpurchase"].DefaultView;
                    dataGridView1.Rows[0].Cells[0].Value = "Total :-";
                    dataGridView1.Rows[0].DefaultCellStyle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
                    dataGridView1.Columns[2].Visible = false;

                    dataGridView1.Rows[0].Frozen = true;
                    dataGridView1.AllowUserToOrderColumns = false;

                }
                if (strFormName == "Stock Rol")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type :" + strproduct + ",Manufacture :" + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("select tblItemMaster.ItemName as [Item Name],QOH,tblItemMaster.ROL from tblItemMaster join vwstock on tblItemMaster.ItemCode=vwstock.[item code] where ROL>=QOH and ROL!=0 order by [Item Name]", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Stock Consolidate")
                {

                    string firstcolumn = "";
                    string sumcolumn = "";
                    label2.Text = "Item Name:" + stritem + ", DATE:" + strFrom /*+ ",TO DATE:" + strTo*/ ;
                    label1.Text = strFormName + " Report";
                    SqlConnection con1 = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString);
                    con1.Open();
                    SqlCommand cmd1 = new SqlCommand("select sum(QtyIn-QtyOut)   from tblStockHistory   where ItemID in (select ItemID from tblItemmaster where  ItemName='" + stritem + "') and VchDate < '" + strFrom + "'", con1);
                    using (SqlDataReader reader = cmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            firstcolumn = reader[0].ToString();
                        }
                    }
                    SqlConnection con2 = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString);
                    con2.Open();
                    SqlCommand cmd2 = new SqlCommand("select sum(QtyIn-QtyOut)   from tblStockHistory   where ItemID in (select ItemID from vwstock join tblItemMaster on vwstock.[Item Code]=tblItemMaster.ItemCode)", con1);
                    using (SqlDataReader reader1 = cmd2.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            sumcolumn = reader1[0].ToString();
                        }
                    }
                    using (SqlConnection con = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(" select * from vwstock order by vchdate", con))
                        {
                            cmd.CommandType = CommandType.Text;
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                using (DataTable dt = new DataTable())
                                {
                                    sda.Fill(dt);
                                    dataGridView1.DataSource = dt;
                                    int sum = 0;
                                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                                    {
                                        sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value);
                                    }

                                    int sum1 = 0;
                                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                                    {
                                        sum1 += Convert.ToInt32(dataGridView1.Rows[i].Cells[8].Value);
                                    }
                                    int rowIdx = dataGridView1.Rows.Count - 1;
                                    DateTime? newdate = null;
                                    dt.Rows.Add(newdate, "", "", "", "", "Total :-", null, sum, sum1);
                                    dt.Rows.Add(newdate, "", "", "", "", "Qty :-", null, (sum - sum1));
                                    if (firstcolumn == "")
                                    {
                                        dt.Rows.Add(newdate, "", "", "", "", "Opening Stock:-", null, "0");

                                    }
                                    else
                                    {
                                        dt.Rows.Add(newdate, "", "", "", "", "Opening Stock", null, firstcolumn);

                                    }
                                    if (sumcolumn == "")
                                    {
                                        dt.Rows.Add(newdate, "", "", "", "", "QOH", null, "0");

                                    }
                                    else
                                    {
                                        dt.Rows.Add(newdate, "", "", "", "", "QOH", null, sumcolumn);
                                    }

                                }
                            }
                        }
                    }



                }
                else if (strFormName == "Stock Value")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock order by [Item Name]", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";
                    string a = "";
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        string columnName = dataGridView1.Columns[i].Name;
                        if (columnName == "QOH" || columnName == "Purchase Rate" || columnName == "Purchase Value" || columnName == "Cost Rate Inclusive" || columnName == "Cost Value")
                        {
                            a = a + "cast(sum([" + columnName + "])as numeric(36,2)) as N" + i + ",";

                        }
                        else
                        {
                            a = a + " Null as N" + i + ",";
                        }

                    }
                    string b = a.Remove(a.Length - 1, 1);

                    string sql = "select * from vwstock  union all SELECT " + b + "  FROM vwstock ORDER BY [Item Name]";
                    SqlDataAdapter da1 = new SqlDataAdapter(sql, DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1, "vwpurchase");
                    dataGridView1.DataSource = ds1.Tables["vwpurchase"].DefaultView;
                    dataGridView1.Rows[0].Cells[0].Value = "Total :-";
                    dataGridView1.Rows[0].DefaultCellStyle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
                    dataGridView1.Columns[2].Visible = false;

                    dataGridView1.Rows[0].Frozen = true;
                    dataGridView1.AllowUserToOrderColumns = false;
                }
                else if (strFormName == "Stock Moq")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("select tblItemMaster.ItemName as [Item Name],QOH,Minqty as MOQ from tblItemMaster join vwstock on tblItemMaster.ItemCode=vwstock.[item code] where Minqty>=QOH AND Minqty !=0 order by [Item Name]", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }

                else if (strFormName == "Stock Zero")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("select tblItemMaster.ItemName as [Item Name],QOH,Minqty as MOQ from tblItemMaster join vwstock on tblItemMaster.ItemCode=vwstock.[item code] where QOH=0 order by [Item Name]", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Fast Moving")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("select  CCName as CostCenter,ItemCode,ItemName,UnitName,sum(QtyOut) as [Sale Qty] from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblVchType on tblStockHistory.VchType=tblVchType.VchType where tblVchType.ParentID=1 group by CCName,ItemCode,ItemName,UnitName,QtyOut order by [Sale Qty] DESC ", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Slow Moving")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("select  CCName as CostCenter,ItemCode,ItemName,UnitName,sum(QtyOut) as [Sale Qty] from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblVchType on tblStockHistory.VchType=tblVchType.VchType where tblVchType.ParentID=1 group by CCName,ItemCode,ItemName,UnitName,QtyOut order by [Sale Qty] ASC ", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Non Moving")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("select  CCName as CostCenter,ItemCode,ItemName,UnitName,sum(QtyOut) as SQty from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblVchType on tblStockHistory.VchType=tblVchType.VchType where tblVchType.ParentID=1 and Qtyout=0 group by CCName,ItemCode,ItemName,UnitName,QtyOut order by tblItemMaster.ItemName", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Supplier Wise ROL")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock where ROL>=QOH AND ROL!=0", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Supplier Items")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock order by QOH ASC", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                }
                else if (strFormName == "Stock Expiry")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type : " + strproduct + ",Manufacture : " + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock order by Expiry ASC", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Stock Movement")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo;
                    SqlDataAdapter da = new SqlDataAdapter(mSql, DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Stock Movement Detail")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type" + strproduct + ",Manufacture" + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter(mSql, DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";

                }
                else if (strFormName == "Stock Movement Item Wise")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type" + strproduct + ",Manufacture" + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter(mSql, DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";
                }

                else if (strFormName == "Repacking Daybook")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type" + strproduct + ",Manufacture" + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock ORDER BY INVDATE,INVNO", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                    label1.Text = strFormName + " Report";
                    dt.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = "Total:-";

                    for (int i = 2; i < dataGridView1.Columns.Count; i++)
                    {
                        int total = 0;
                        for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                        {
                            total += Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value);
                        }
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[i].Value = total;
                    }
                }
                else if (strFormName == "Finished Goods")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type" + strproduct + ",Manufacture" + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                    label1.Text = strFormName + " Report";
                    dt.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = "Total:-";

                    for (int i = 1; i < dataGridView1.Columns.Count; i++)
                    {
                        int total = 0;
                        for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                        {
                            total += Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value);
                        }
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[i].Value = total;
                    }
                }

                else if (strFormName == "Raw Material")
                {
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Prduct Type" + strproduct + ",Manufacture" + strmnf;
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwstock", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                    label1.Text = strFormName + " Report";
                    dt.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = "Total:-";

                    for (int i = 1; i < dataGridView1.Columns.Count; i++)
                    {
                        int total = 0;
                        for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                        {
                            total += Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value);
                        }
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[i].Value = total;
                    }
                }
                else if (strFormName == "Ledger")
                {

                    string sqlSupplier = "";
                    if (strLedger != "")
                    {
                        sqlSupplier = " L1.LAliasName = '" + strLedger + "' AND ";
                    }
                    DateTime FD = Convert.ToDateTime(strFrom);
                    DateTime TD = Convert.ToDateTime(strTo);



                    using (SqlConnection con = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT dbo.tblVoucher.VchNo AS VOUCHER_NO,  " +
                        "  dbo.tblVoucher.VchDate AS VOUCHER_DATE, dbo.tblAccountGroup.AccountGroup AS AccountGroup," +
                        " L1.LAliasName as LEDGER_NAME, dbo.tblVchType.VchType AS VOUCHER_TYPE," +
                        " dbo.tblVoucher.Mynarration AS NARRATION, dbo.tblEmployee.Name AS STAFF_NAME," +
                        " SUM(isnull(dbo.tblVoucher.AmountD, 0)) AS DEBIT, SUM(isnull(DBO.TBLVOUCHER.AMOUNTC, 0)) AS CREDIT" +
                        " FROM dbo.tblAccountGroup INNER JOIN dbo.tblLedger as L1 ON dbo.tblAccountGroup.AccountGroupID = L1.AccountGroupID" +
                        "  INNER JOIN dbo.tblVoucher ON L1.LID = dbo.tblVoucher.LedgerID INNER JOIN " +
                        "   dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID" +
                        " LEFT OUTER JOIN dbo.tblEmployee ON dbo.tblVoucher.SalesManID = dbo.tblEmployee.EmpID" +
                        " WHERE ISNULL(tblAccountGroup.activestatus, 1) = 1 and tblVoucher.Optional = 0 AND " + sqlSupplier + " " +
                        "  vchdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "'  AND '" + TD.ToString("dd-MMM-yyyy") + "'" +
                        " GROUP BY dbo.tblVoucher.VchNo, dbo.tblVoucher.VchDate, dbo.tblAccountGroup.AccountGroup," +
                        " L1.LAliasName,dbo.tblVchType.VchType, dbo.tblVoucher.Mynarration, dbo.tblEmployee.Name" +
                        " ORDER BY dbo.tblVoucher.VchDate,  dbo.tblVoucher.VchNo ", con))
                        //having(SUM(isnull(dbo.tblVoucher.AmountD, 0)) - SUM(isnull(dbo.tblVoucher.AmountC, 0))) <> 0 
                        {
                            cmd.CommandType = CommandType.Text;
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                using (DataTable dt = new DataTable())
                                {
                                    sda.Fill(dt);
                                    dataGridView1.DataSource = dt;

                                    int sum = 0;
                                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                                    {
                                        sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value);
                                    }

                                    int sum1 = 0;
                                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                                    {
                                        sum1 += Convert.ToInt32(dataGridView1.Rows[i].Cells[8].Value);
                                    }
                                    int rowIdx = dataGridView1.Rows.Count - 1;
                                    DateTime? newdate = null;
                                    dt.Rows.Add("", newdate, "", "", "", "", "Total", sum, sum1);

                                    if (sum - sum1 < 0)
                                    {
                                        dt.Rows.Add("", newdate, "", "", "", "", "Balance : " + Math.Abs(sum - sum1) + " CR");
                                    }
                                    else
                                    {
                                        dt.Rows.Add("", newdate, "", "", "", "", "Balance : " + Math.Abs(sum1 - sum) + " DR");
                                    }

                                }
                            }
                        }
                    }

                    int j = dataGridView1.Rows.Count - 1;
                    dataGridView1.Rows[j].DefaultCellStyle.Font = new System.Drawing.Font("Roboto", 9F, FontStyle.Bold);

                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",LEDGER :" + strLedger;

                }
                else if (strFormName == "Account Group Wise")
                {

                    DateTime FD = Convert.ToDateTime(strFrom);
                    DateTime TD = Convert.ToDateTime(strTo);


                    using (SqlConnection con = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(" SELECT dbo.tblVoucher.VchNo AS VOUCHER_NO, " +
                        "  dbo.tblVoucher.VchDate AS VAUCHER_DATE, dbo.tblAccountGroup.AccountGroup AS ACCOUNT_GROUP," +
                        " L1.LAliasName as LEDGER_NAME, dbo.tblVchType.VchType AS VOUCHER_TYPE," +
                        " dbo.tblVoucher.Mynarration AS NARRATION, dbo.tblEmployee.Name AS STAFF_NAME," +
                        " SUM(isnull(dbo.tblVoucher.AmountD, 0)) AS DEBIT, SUM(isnull(DBO.TBLVOUCHER.AMOUNTC, 0)) AS CREDIT" +
                        " FROM dbo.tblAccountGroup INNER JOIN dbo.tblLedger as L1 ON dbo.tblAccountGroup.AccountGroupID = L1.AccountGroupID" +
                        "  INNER JOIN dbo.tblVoucher ON L1.LID = dbo.tblVoucher.LedgerID INNER JOIN " +
                        "   dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID" +
                        " LEFT OUTER JOIN dbo.tblEmployee ON dbo.tblVoucher.SalesManID = dbo.tblEmployee.EmpID" +
                        " WHERE ISNULL(tblAccountGroup.activestatus, 1) = 1 and tblVoucher.Optional = 0 AND L1.AccountGroupID in (" + strids + ")" +
                        " And vchdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "'  AND '" + TD.ToString("dd-MMM-yyyy") + "'" +
                        " GROUP BY dbo.tblVoucher.VchNo, dbo.tblVoucher.VchDate, dbo.tblAccountGroup.AccountGroup," +
                        " L1.LAliasName,dbo.tblVchType.VchType, dbo.tblVoucher.Mynarration, dbo.tblEmployee.Name" +
                        " having(SUM(isnull(dbo.tblVoucher.AmountD, 0)) - SUM(isnull(dbo.tblVoucher.AmountC, 0))) <> 0 ORDER BY dbo.tblVoucher.VchDate,  dbo.tblVoucher.VchNo ", con))
                        {
                            cmd.CommandType = CommandType.Text;
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                using (DataTable dt = new DataTable())
                                {
                                    sda.Fill(dt);
                                    dataGridView1.DataSource = dt;

                                    int sum = 0;
                                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                                    {
                                        sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value);
                                    }

                                    int sum1 = 0;
                                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                                    {
                                        sum1 += Convert.ToInt32(dataGridView1.Rows[i].Cells[8].Value);
                                    }
                                    int rowIdx = dataGridView1.Rows.Count - 1;
                                    DateTime? newdate = null;
                                    dt.Rows.Add("", newdate, "", "", "", "", "Total", sum, sum1);

                                    if (sum - sum1 < 0)
                                    {
                                        dt.Rows.Add("", newdate, "", "", "", "", "Balance : " + Math.Abs(sum - sum1) + " CR");
                                    }
                                    else
                                    {
                                        dt.Rows.Add("", newdate, "", "", "", "", "Balance : " + Math.Abs(sum1 - sum) + " DR");
                                    }
                                }
                            }
                        }
                    }

                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",LEDGER :" + strLedger;
                }
                else if (strFormName == "Supplier Outstanding")
                {

                    int amt = Int16.Parse(stramt);

                    SqlDataAdapter da = new SqlDataAdapter("SELECT L1.LAliasName as LEDGER_NAME,L1.Address as ADDRESS,MobileNo as MOBILE_NO,ABS(SUM(AmountD)-SUM(AmountC))AS BALANCE, CASE WHEN ((SUM(AmountD)-SUM(AmountC))> '0') THEN 'DR' ELSE 'CR' END AS  _  FROM dbo.tblAccountGroup INNER JOIN dbo.tblLedger as L1 ON dbo.tblAccountGroup.AccountGroupID = L1.AccountGroupID INNER JOIN dbo.tblVoucher ON L1.LID = dbo.tblVoucher.LedgerID INNER JOIN    dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID LEFT OUTER JOIN dbo.tblEmployee ON dbo.tblVoucher.SalesManID = dbo.tblEmployee.EmpID WHERE ISNULL(tblAccountGroup.activestatus, 1) = 1 and tblVoucher.Optional = 0  AND  L1.AccountGroupID = 11 GROUP BY L1.LAliasName, L1.Address, MobileNo having(SUM(AmountD) - SUM(AmountC)) >=" + amt + "", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";
                    label2.Text = "Above Amount: " + amt + ",Area :" + strArea;
                }
                else if (strFormName == "Customer Outstanding")
                {

                    int amt = Int16.Parse(stramt);
                    SqlDataAdapter da = new SqlDataAdapter("SELECT L1.LAliasName as LEDGER_NAME,L1.Address as ADDRESS,MobileNo as MOBILE_NO,ABS(SUM(AmountD)-SUM(AmountC)) AS BALANCE, CASE WHEN ((SUM(AmountD)-SUM(AmountC))> '0') THEN 'DR' ELSE 'CR' END AS _  FROM dbo.tblAccountGroup INNER JOIN dbo.tblLedger as L1 ON dbo.tblAccountGroup.AccountGroupID = L1.AccountGroupID INNER JOIN dbo.tblVoucher ON L1.LID = dbo.tblVoucher.LedgerID INNER JOIN    dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID LEFT OUTER JOIN dbo.tblEmployee ON dbo.tblVoucher.SalesManID = dbo.tblEmployee.EmpID WHERE ISNULL(tblAccountGroup.activestatus, 1) = 1 and tblVoucher.Optional = 0  AND  L1.AccountGroupID = 10 and Area='" + strArea + "' GROUP BY L1.LAliasName, L1.Address, MobileNo having(SUM(AmountD) - SUM(AmountC)) >=" + amt + " ", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "vwstock");
                    dataGridView1.DataSource = ds.Tables["vwstock"].DefaultView;
                    label1.Text = strFormName + " Report";
                    label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",LEDGER :" + strLedger;

                }
                else if (strFormName == "Cash Desk")
                {

                    SqlDataAdapter da1 = new SqlDataAdapter(" select  string_agg (PaymentType, ',') as PaymentType  from tblCashDeskMaster", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataTable dt3 = new DataTable();
                    da1.Fill(dt3);
                    SqlDataAdapter da = new SqlDataAdapter("select * from vwpurchase  PIVOT(AVG(amount) FOR PaymentType in (" + dt3.Rows[0]["PaymentType"].ToString() + ",CREDIT)) AS PivotTable  order by ID", DigiposZen.Properties.Settings.Default.ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        double[] Total = new double[dt.Columns.Count];

                        dataGridView1.Columns.Clear();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dataGridView1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                            if (i > 5)
                            {
                                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                        }
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dataGridView1.Rows.Add();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (j > 5)
                                {
                                    dataGridView1[j, i].Value = Comm.FormatAmt(Comm.ToDouble(dt.Rows[i][j]), AppSettings.CurrDecimalFormat);
                                    Total[j] += Comm.ToDouble(dataGridView1[j, i].Value);
                                }
                                else
                                {
                                    dataGridView1[j, i].Value = dt.Rows[i][j];
                                }
                            }
                        }

                        dataGridView1.Rows.Add();
                        dataGridView1[5, dataGridView1.RowCount - 1].Value = "Total : ";
                        for (int i = 6; i < dt.Columns.Count; i++)
                        {
                            dataGridView1[i, dataGridView1.RowCount - 1].Value = Comm.FormatAmt(Comm.ToDouble(Total[i]), AppSettings.CurrDecimalFormat);
                        }

                        //     dataGridView1.DataSource = dt;
                        //     dataGridView1.Columns["ID"].Visible = false;

                        // label1.Text = strFormName + " Report";
                        // label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",LEDGER :" + strLedger;
                        // foreach (DataGridViewRow row in dataGridView1.Rows)
                        // {
                        //     foreach (DataGridViewCell item in row.Cells)
                        //     {
                        //         if (item.Value == null || item.Value == DBNull.Value || String.IsNullOrWhiteSpace(item.Value.ToString()))
                        //         {
                        //             item.Value = "0";
                        //         }
                        //     }
                        // }
                        // dt.Rows.Add();


                        //dataGridView1.Rows[dataGridView1.Rows.Count-1].Cells[0].Value = "Total:-";

                        // for (int i = 6; i < dataGridView1.Columns.Count; i++)
                        // {
                        //     int total = 0;
                        //     for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                        //     {
                        //         total += Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value);
                        //     }
                        //     dataGridView1.Rows[dataGridView1.Rows.Count-1 ].Cells[i].Value = total;
                        // }
                    }
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        string columnName = dataGridView1.Columns[i].Name;



                        this.dataGridView1.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            
        }

        private void FillReportManual(DataTable dt, string NumberFormatCols = "", string DateFormatCols = "", string SumCols = "")
        {
            try
            {
                NumberFormatCols = "," + NumberFormatCols + ",";
                DateFormatCols = "," + DateFormatCols + ",";
                SumCols = "," + SumCols + ",";

                if (dt.Rows.Count > 0)
                {
                    double[] Total = new double[dt.Columns.Count];

                    dataGridView1.Columns.Clear();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dataGridView1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        if (i > 5)
                        {
                            dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dataGridView1.Rows.Add();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (j > 5)
                            {
                                dataGridView1[j, i].Value = Comm.FormatAmt(Comm.ToDouble(dt.Rows[i][j]), AppSettings.CurrDecimalFormat);
                                Total[j] += Comm.ToDouble(dataGridView1[j, i].Value);
                            }
                            else
                            {
                                dataGridView1[j, i].Value = dt.Rows[i][j];
                            }
                        }
                    }

                    dataGridView1.Rows.Add();
                    dataGridView1[5, dataGridView1.RowCount - 1].Value = "Total : ";
                    for (int i = 6; i < dt.Columns.Count; i++)
                    {
                        dataGridView1[i, dataGridView1.RowCount - 1].Value = Comm.FormatAmt(Comm.ToDouble(Total[i]), AppSettings.CurrDecimalFormat);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (dataGridView1.Rows.Count > 0)
                {
                    if (strFormName == "Sales Daybook" || strFormName == "Sales Detail Daybook" || strFormName == "Sales Hsncode Wise" || strFormName == "Sales Item Wise" || strFormName == "Sales Tax split" || strFormName == "Purchase Daybook" || strFormName == "Purchase Detail Daybook" || strFormName == "Purchase Hsncode Wise" || strFormName == "Purchase Item Wise" || strFormName == "Purchase Tax split" || strFormName == "Sales Discount" || strFormName == "Purchase Return Daybook" || strFormName == "Purchase Return Daybook Details" || strFormName == "Purchase Return Hsncode Wise" || strFormName == "Purchase Return Tax split" || strFormName == "Purchase Return Item Wise" || strFormName == "Sales Return Daybook" || strFormName == "Sales Return Daybook Details" || strFormName == "Sales Return Hsncode Wise" || strFormName == "Sales Return Item Wise" || strFormName == "Sales Return Tax split" || strFormName == "Sales Return Discount" || strFormName == "Delivery Note Daybook" || strFormName == "Delivery Note Detail Daybook" || strFormName == "Delivery Note Item Wise" || strFormName == "Receipt Note Daybook" || strFormName == "Receipt Note Detail Daybook" || strFormName == "Receipt Note Item Wise")
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "CSV (*.csv)|*.csv";
                        sfd.FileName = ".csv";
                        bool fileError = false;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            if (File.Exists(sfd.FileName))
                            {
                                try
                                {
                                    File.Delete(sfd.FileName);
                                }
                                catch (IOException ex)
                                {
                                    fileError = true;
                                    MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                                }
                            }
                            if (!fileError)
                            {
                                try
                                {
                                    int columnCount = dataGridView1.Columns.Count;
                                    string columnNames = "";
                                    string[] outputCsv = new string[dataGridView1.Rows.Count + 3];
                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        columnNames += dataGridView1.Columns[i].HeaderText.ToString() + ",";
                                    }

                                    outputCsv[0] = strFormName + " Report";
                                    string a = "FROM DATE:" + strFrom + " ->TO DATE:" + strTo + "->Cost Center:" + strcost + "->Voucher Type:" + strVchtype;
                                    string b = a.Replace(",", ";");
                                    outputCsv[1] = b;
                                    outputCsv[2] += columnNames;

                                    for (int i = 3; (i) < dataGridView1.Rows.Count + 2; i++)
                                    {
                                        for (int j = 0; j < columnCount; j++)
                                        {
                                            outputCsv[i] += dataGridView1.Rows[i - 2].Cells[j].Value.ToString().Replace(",", " ").Replace("\r"," ").Replace("\n", " ") + ",";
                                        }

                                    }

                                    for (int j = 0; j < columnCount; j++)
                                    {
                                        outputCsv[dataGridView1.Rows.Count + 2] += dataGridView1.Rows[0].Cells[j].Value.ToString() + ",";
                                    }

                                    File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                                    MessageBox.Show("Data Exported Successfully !!!", "Info");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error :" + ex.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "CSV (*.csv)|*.csv";
                        sfd.FileName = ".csv";
                        bool fileError = false;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            if (File.Exists(sfd.FileName))
                            {
                                try
                                {
                                    File.Delete(sfd.FileName);
                                }
                                catch (IOException ex)
                                {
                                    fileError = true;
                                    MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                                }
                            }
                            if (!fileError)
                            {
                                try
                                { 
                                    int columnCount = dataGridView1.Columns.Count;
                                    string columnNames = "";
                                    string[] outputCsv = new string[dataGridView1.Rows.Count + 3];
                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        columnNames += dataGridView1.Columns[i].HeaderText.ToString() + ",";
                                    }

                                    outputCsv[0] = strFormName + " Report";
                                    string a = "FROM DATE:" + strFrom + " ->TO DATE:" + strTo + "->Cost Center:" + strcost + "->Voucher Type:" + strVchtype;
                                    string b = a.Replace(",", ";");
                                    outputCsv[1] = b;
                                    outputCsv[2] += columnNames;

                                    for (int i = 1; (i) < dataGridView1.Rows.Count + 1; i++)
                                    {
                                        for (int j = 0; j < columnCount; j++)
                                        {
                                            outputCsv[i+2] += dataGridView1.Rows[i -1].Cells[j].Value.ToString().Replace(",", " ") + ",";
                                        }

                                    }

                                    File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                                    MessageBox.Show("Data Exported Successfully !!!", "Info");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error :" + ex.Message);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No Record To Export !!!", "Info");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        
        }

       

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void frmReportView1_ResizeEnd(object sender, EventArgs e)
        {
            dataGridView1.Width = this.Width - 10;
            dataGridView1.Height = this.Height - 20;
        }
    
    }

}
