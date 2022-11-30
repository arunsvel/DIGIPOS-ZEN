using System;
using System.Data;
using System.Windows.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using InventorSync.Forms;
using InventorSync.JsonClass;
using System.Data.SqlClient;
using InventorSync.InventorBL.Accounts;
using Microsoft.VisualBasic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace InventorSync
{
    public partial class frmDashBoard : Form
    {
        public frmDashBoard()
        {
            InitializeComponent();
        }
        string constr = Properties.Settings.Default.ConnectionString; // @"Data Source = GAMERADICTION\DIGIPOS; Initial Catalog = DigiposDemo; User ID = sa; Password =#infinitY@279";

        private void frmDashBoard_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
            chart3.Text = "loadsales";
            SqlConnection con1 = new SqlConnection(constr);
            con1.Open();
            SqlCommand cmd1 = new SqlCommand("select  MOP,sum(BillAmt) [BillAmt] from tblSales Group by MOP", con1);
            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
            DataSet st1 = new DataSet();
            sda1.Fill(st1, "BillAmt");


            chart4.DataSource = st1.Tables["BillAmt"];
            chart4.Series[0].XValueMember = "MOP";
            chart4.Series[0].YValueMembers = "BillAmt";
            this.chart4.Titles.Add("MOP Sales");
            chart4.Series[0].ChartType = SeriesChartType.Pie;
            chart4.Series[0].IsValueShownAsLabel = true;
            chart4.ChartAreas[0].Area3DStyle.Enable3D = true;
            con1.Close();
            string query1 = "select  tblSales.VchType,sum(BillAmt) [Total] from tblSales join tblVchType on tblSales.VchType=tblVchType.VchType where tblVchType.ParentID=1 Group by tblSales.VchType ";
            DataTable dt1 = GetData(query1);

            string[] x1 = (from q in dt1.AsEnumerable()
                           orderby q.Field<string>("VchType") ascending
                           select q.Field<string>("VchType")).ToArray();

            double[] y1 = (from q in dt1.AsEnumerable()
                           orderby q.Field<string>("VchType") ascending
                           select q.Field<double>("Total")).ToArray();

            chart3.Series[0].ChartType = SeriesChartType.Column;
            chart3.Series[0].Points.DataBindXY(x1, y1);
            chart3.ChartAreas[0].Area3DStyle.Enable3D = false;
            button1.Text = DateTime.Now.ToString();
            SqlConnection Conn = new SqlConnection(constr);
            SqlCommand Comm1 = new SqlCommand("select count(InvId),sum(BillAmt) from tblSales where InvDate='2022-09-29 00:00:00.000' and VchTypeID=1", Conn);
            Conn.Open();
            SqlDataReader DR1 = Comm1.ExecuteReader();
            if (DR1.Read())
            {
                if (DR1.GetValue(1).ToString() != "")
                {
                    string fare = DR1.GetValue(1).ToString();
                    decimal parsed = decimal.Parse(fare, CultureInfo.InvariantCulture);
                    CultureInfo hindi = new CultureInfo("hi-IN");
                    string text = string.Format(hindi, "{0:c}", parsed);

                    btnSales.Text = "Sales Count:" + DR1.GetValue(0).ToString() + "\nAmount:" + text;
                }
            }
            Conn.Close();

            SqlConnection Conn1 = new SqlConnection(constr);
            SqlCommand Comm2 = new SqlCommand("select count(InvId),sum(BillAmt) from tblPurchase where VchTypeID=2", Conn1);
            Conn1.Open();
            SqlDataReader DR2 = Comm2.ExecuteReader();
            if (DR2.Read())
            {
                if (DR2.GetValue(1).ToString() != "0")
                {
                    string fare = DR2.GetValue(1).ToString();
                    decimal parsed = decimal.Parse(fare, CultureInfo.InvariantCulture);
                    CultureInfo hindi = new CultureInfo("hi-IN");
                    string text = string.Format(hindi, "{0:c}", parsed);
                    btnPurchase.Text = "Purchase Count:" + DR2.GetValue(0).ToString() + "\nAmount:" + text;
                }
            }
            Conn1.Close();

            SqlConnection Conn2 = new SqlConnection(constr);
            SqlCommand Comm3 = new SqlCommand("select count(InvId),sum(BillAmt) from tblSales where InvDate='2022-09-29 00:00:00.000' and VchTypeID=3", Conn2);
            Conn2.Open();
            SqlDataReader DR3 = Comm3.ExecuteReader();
            if (DR3.Read())
            {
                if (DR3.GetValue(1).ToString() != "")
                        
                {
                    string fare = DR3.GetValue(1).ToString();
                    decimal parsed = decimal.Parse(fare, CultureInfo.InvariantCulture);
                    CultureInfo hindi = new CultureInfo("hi-IN");
                    string text = string.Format(hindi, "{0:c}", parsed);
                    btnSalesReturn.Text = "SalesReturn Count:" + DR3.GetValue(0).ToString() + "\nAmount:" + text;
                }
            }
            Conn2.Close();

            SqlConnection Conn3 = new SqlConnection(constr);
            SqlCommand Comm4 = new SqlCommand("select count(InvId),sum(BillAmt) from tblPurchase where InvDate='2022-09-29 00:00:00.000' and VchTypeID=4", Conn3);
            Conn3.Open();
            SqlDataReader DR4 = Comm4.ExecuteReader();
            if (DR4.Read())
            {
                if (DR4.GetValue(1).ToString() != "")

                {
                    string fare = DR4.GetValue(1).ToString();
                    decimal parsed = decimal.Parse(fare, CultureInfo.InvariantCulture);
                    CultureInfo hindi = new CultureInfo("hi-IN");
                    string text = string.Format(hindi, "{0:c}", parsed);
                    btnPurchaseReturn.Text = "Purchase Return Count:" + DR4.GetValue(0).ToString() + "\nAmount:" + text;
                }
            }
            Conn3.Close();

            SqlConnection Conn4 = new SqlConnection(constr);
            SqlCommand Comm5 = new SqlCommand("select count(VchDate),sum(AmountD) from tblVoucher where VchDate='2022-09-14 00:00:00.000' and VchTypeID=7", Conn4);
            Conn4.Open();
            SqlDataReader DR5 = Comm5.ExecuteReader();
            if (DR5.Read())
            {
                if (DR5.GetValue(1).ToString() != "")

                {
                    string fare = DR5.GetValue(1).ToString();
                    decimal parsed = decimal.Parse(fare, CultureInfo.InvariantCulture);
                    CultureInfo hindi = new CultureInfo("hi-IN");
                    string text = string.Format(hindi, "{0:c}", parsed);
                    btnReciept.Text = "Receipt Count:" + DR5.GetValue(0).ToString() + "\nAmount:" + text;
                }
            }
            Conn4.Close();

            SqlConnection Conn5 = new SqlConnection(constr);
            SqlCommand Comm6 = new SqlCommand("select count(VchDate),sum(AmountC) from tblVoucher where VchDate='2022-09-22 00:00:00.000' and VchTypeID=8", Conn5);
            Conn5.Open();
            SqlDataReader DR6 = Comm6.ExecuteReader();
            if (DR6.Read())
            {
                if (DR6.GetValue(1).ToString() != "")
                {
                    string fare = DR6.GetValue(1).ToString();
                    decimal parsed = decimal.Parse(fare, CultureInfo.InvariantCulture);
                    CultureInfo hindi = new CultureInfo("hi-IN");
                    string text = string.Format(hindi, "{0:c}", parsed);
                    btnPayment.Text = "Payment Count:" + DR6.GetValue(0).ToString() + "\nAmount:" + text;
                }
            }
            Conn3.Close();
        }
        private static DataTable GetData(string query)
        {
            string constr = Properties.Settings.Default.ConnectionString; //@"Data Source = //GAMERADICTION\DIGIPOS; Initial Catalog = DigiposDemo; User ID = sa; Password =#infinitY@279";

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show(" **YES** Refresh The Window \n **NO** Close The Window", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                button1.Text = DateTime.Now.ToString();
                this.Refresh();

            }
            else if (dialogResult == DialogResult.No)
            {
                this.Close();
            }

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tbcSales_Click(object sender, EventArgs e)
        {
            button1.Text = DateTime.Now.ToString();


            if (tbcSales.SelectedTab.Text == "SALES")
            {
                if (chart3.Text != "loadsales")
                {
                    chart3.Text = "loadsales";
                    SqlConnection con1 = new SqlConnection(constr);
                    con1.Open();
                    SqlCommand cmd1 = new SqlCommand("select  MOP,sum(BillAmt) [BillAmt] from tblSales Group by MOP", con1);
                    SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                    DataSet st1 = new DataSet();
                    sda1.Fill(st1, "BillAmt");


                    chart4.DataSource = st1.Tables["BillAmt"];
                    chart4.Series[0].XValueMember = "MOP";
                    chart4.Series[0].YValueMembers = "BillAmt";
                    this.chart4.Titles.Add("MOP Sales");
                    chart4.Series[0].ChartType = SeriesChartType.Pie;
                    chart4.Series[0].IsValueShownAsLabel = true;
                    chart4.ChartAreas[0].Area3DStyle.Enable3D = true;
                    con1.Close();
                    string query1 = "select  tblSales.VchType,sum(BillAmt) [Total] from tblSales join tblVchType on tblSales.VchType=tblVchType.VchType where tblVchType.ParentID=1 Group by tblSales.VchType ";
                    DataTable dt1 = GetData(query1);

                    string[] x1 = (from q in dt1.AsEnumerable()
                                   orderby q.Field<string>("VchType") ascending
                                   select q.Field<string>("VchType")).ToArray();

                    double[] y1 = (from q in dt1.AsEnumerable()
                                   orderby q.Field<string>("VchType") ascending
                                   select q.Field<double>("Total")).ToArray();

                    chart3.Series[0].ChartType = SeriesChartType.Column;
                    chart3.Series[0].Points.DataBindXY(x1, y1);
                    chart3.ChartAreas[0].Area3DStyle.Enable3D = false;
                }
            }

            if (tbcSales.SelectedTab.Text == "PURCHASE")
            {
                if (chart1.Text != "loadpurchase")
                {
                    chart1.Text = "loadpurchase";
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select  MOP,sum(BillAmt) [BillAmt] from tblpurchase join tblVchType on tblpurchase.VchType=tblVchType.VchType where tblVchType.ParentID=2 Group by MOP", con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet st = new DataSet();
                    sda.Fill(st, "BillAmt");


                    chart2.DataSource = st.Tables["BillAmt"];
                    chart2.Series[0].XValueMember = "MOP";
                    chart2.Series[0].YValueMembers = "BillAmt";
                    this.chart2.Titles.Add("MOP Purchase");
                    chart2.Series[0].ChartType = SeriesChartType.Pie;
                    chart2.Series[0].IsValueShownAsLabel = true;
                    chart2.ChartAreas[0].Area3DStyle.Enable3D = true;
                    con.Close();
                    string query = "select  tblpurchase.VchType,sum(BillAmt) [Total] from tblpurchase join tblVchType on tblpurchase.VchType=tblVchType.VchType where tblVchType.ParentID=2 Group by tblpurchase.VchType ";
                    DataTable dt = GetData(query);

                    string[] x = (from p in dt.AsEnumerable()
                                  orderby p.Field<string>("VchType") ascending
                                  select p.Field<string>("VchType")).ToArray();

                    double[] y = (from p in dt.AsEnumerable()
                                  orderby p.Field<string>("VchType") ascending
                                  select p.Field<double>("Total")).ToArray();

                    chart1.Series[0].ChartType = SeriesChartType.Column;
                    chart1.Series[0].Points.DataBindXY(x, y);
                    chart1.ChartAreas[0].Area3DStyle.Enable3D = false;

                }
            }
            if (tbcSales.SelectedTab.Text == "RETURNS")
            {
                if (chart5.Text != "loadreturns")
                {
                    chart5.Text = "loadreturns";
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select  MOP,sum(BillAmt) [BillAmt] from tblpurchase join tblVchType on tblpurchase.VchType=tblVchType.VchType where tblVchType.ParentID=4  Group by MOP", con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet st = new DataSet();
                    sda.Fill(st, "BillAmt");
                    chart5.DataSource = st.Tables["BillAmt"];
                    chart5.Series[0].XValueMember = "MOP";
                    chart5.Series[0].YValueMembers = "BillAmt";
                    this.chart5.Titles.Add("MOP Purchase Return");
                    chart5.Series[0].ChartType = SeriesChartType.Pie;
                    chart5.Series[0].IsValueShownAsLabel = true;
                    chart5.ChartAreas[0].Area3DStyle.Enable3D = true;
                    con.Close();
                    string query = "select  tblpurchase.VchType,sum(BillAmt) [Total] from tblpurchase join tblVchType on tblpurchase.VchType=tblVchType.VchType where  tblVchType.ParentID=4 Group by tblpurchase.VchType ";
                    DataTable dt = GetData(query);

                    string[] x = (from p in dt.AsEnumerable()
                                  orderby p.Field<string>("VchType") ascending
                                  select p.Field<string>("VchType")).ToArray();

                    double[] y = (from p in dt.AsEnumerable()
                                  orderby p.Field<string>("VchType") ascending
                                  select p.Field<double>("Total")).ToArray();

                    chart6.Series[0].ChartType = SeriesChartType.Column;
                    chart6.Series[0].Points.DataBindXY(x, y);
                    chart6.ChartAreas[0].Area3DStyle.Enable3D = false;


                    SqlConnection con1 = new SqlConnection(constr);
                    con1.Open();
                    SqlCommand cmd1 = new SqlCommand("select  MOP,sum(BillAmt) [BillAmt] from tblsales join tblVchType on tblsales.VchType=tblVchType.VchType where tblVchType.ParentID=3  Group by MOP", con1);
                    SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                    DataSet st1 = new DataSet();
                    sda1.Fill(st1, "BillAmt");


                    chart7.DataSource = st1.Tables["BillAmt"];
                    chart7.Series[0].XValueMember = "MOP";
                    chart7.Series[0].YValueMembers = "BillAmt";
                    this.chart7.Titles.Add("MOP Sales Return");
                    chart7.Series[0].ChartType = SeriesChartType.Pie;
                    chart7.Series[0].IsValueShownAsLabel = true;
                    chart7.ChartAreas[0].Area3DStyle.Enable3D = true;
                    con.Close();
                    string query1 = "select  tblsales.VchType,sum(BillAmt) [Total] from tblsales join tblVchType on tblsales.VchType=tblVchType.VchType where tblVchType.ParentID=3 Group by tblsales.VchType ";
                    DataTable dt1 = GetData(query1);

                    string[] x1 = (from p in dt1.AsEnumerable()
                                   orderby p.Field<string>("VchType") ascending
                                   select p.Field<string>("VchType")).ToArray();

                    double[] y1 = (from p in dt1.AsEnumerable()
                                   orderby p.Field<string>("VchType") ascending
                                   select p.Field<double>("Total")).ToArray();

                    chart8.Series[0].ChartType = SeriesChartType.Column;
                    chart8.Series[0].Points.DataBindXY(x1, y1);
                    chart8.ChartAreas[0].Area3DStyle.Enable3D = false;
                }
            }


            if (tbcSales.SelectedTab.Text == "CUSTOMER ANALYSIS")
            {
                SqlConnection Conn1 = new SqlConnection(constr);
                SqlCommand Comm2 = new SqlCommand("select count(InvId) from tblSales where party !='' and VchTypeID=1 and InvDate='2022-09-29 00:00:00.000' ", Conn1);
                Conn1.Open();
                SqlDataReader DR2 = Comm2.ExecuteReader();
                if (DR2.Read())
                {
                    btnTotalNewCustomer.Text = "New customer : " + DR2.GetValue(0).ToString();
                }
                Conn1.Close();
                SqlDataAdapter da = new SqlDataAdapter("select Party as [Customer Name] from tblSales where party !='' and VchTypeID=1 and InvDate='2022-09-29 00:00:00.000' ", constr);
                DataSet ds = new DataSet();
                da.Fill(ds, "vwpurchase");
                dataGridView1.DataSource = ds.Tables["vwpurchase"].DefaultView;
                dataGridView1.Columns[0].Width = 300;
                SqlDataAdapter da1 = new SqlDataAdapter("select PartyCode as [Customer Name] from tblSales where party !='' and VchTypeID=1 and InvDate='2022-09-29 00:00:00.000' ", constr);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1, "vwpurchase");
                dataGridView2.DataSource = ds1.Tables["vwpurchase"].DefaultView;
                dataGridView2.Columns[0].Width = 300;

            }
            if (tbcSales.SelectedTab.Text == "SUPPLIER ANALYSIS")
            {
                SqlConnection Conn1 = new SqlConnection(constr);
                SqlCommand Comm2 = new SqlCommand("select count(InvId) from tblpurchase where party !='' and VchTypeID=2  ", Conn1);
                Conn1.Open();
                SqlDataReader DR2 = Comm2.ExecuteReader();
                if (DR2.Read())
                {
                    btnTotalNewSupplier.Text = "New Supplier : " + DR2.GetValue(0).ToString();
                }
                Conn1.Close();
                SqlDataAdapter da = new SqlDataAdapter("select Party as [Supplier Name] from tblpurchase where party !='' and VchTypeID=2  ", constr);
                DataSet ds = new DataSet();
                da.Fill(ds, "vwpurchase");
                dataGridView4.DataSource = ds.Tables["vwpurchase"].DefaultView;
                dataGridView4.Columns[0].Width = 300;
                SqlDataAdapter da1 = new SqlDataAdapter("select PartyCode as [suppler Name] from tblpurchase where VchTypeID=2 ", constr);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1, "vwpurchase");
                dataGridView3.DataSource = ds1.Tables["vwpurchase"].DefaultView;
                dataGridView3.Columns[0].Width = 300;

            }
            if (tbcSales.SelectedTab.Text == "PROFIT ANALYSIS")
            {
                SqlDataAdapter da = new SqlDataAdapter("select ItemName,tblStock.SRate1,tblStock.PRate,tblStock.SRate1-tblStock.PRate as [Profit Amt] from tblStock join tblItemMaster on tblStock.ItemID=tblItemMaster.ItemID  order by  [Profit Amt] desc", constr);
                DataSet ds = new DataSet();
                da.Fill(ds, "vwpurchase");
                dataGridView11.DataSource = ds.Tables["vwpurchase"].DefaultView;
                dataGridView11.Columns[0].Width = 200;
                dataGridView11.Columns[1].Width = 65;
                dataGridView11.Columns[2].Width = 65;
                dataGridView11.Columns[3].Width = 50;

            }

            if (tbcSales.SelectedTab.Text == "ITEM ANALYSIS")
            {
                
                    try
                    {
                        string Sql = "DROP VIEW vwstock";
                        SqlConnection conn = new SqlConnection(constr);
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(Sql, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch
                    {

                    }
                
                string Sql2 = "create view vwstock as select CCName as CostCenter,ItemCode,ItemName,UnitName,ROUND(sum(QtyIn-QtyOut),2) as QOH from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID   group by CCName,ItemCode,ItemName,UnitName";
                SqlConnection conn2 = new SqlConnection(constr);
                conn2.Open();
                SqlCommand cmd2 = new SqlCommand(Sql2, conn2);
                cmd2.ExecuteNonQuery();
                SqlDataAdapter da2 = new SqlDataAdapter("select tblItemMaster.ItemName,QOH,Minqty as MOQ from tblItemMaster join vwstock on tblItemMaster.ItemCode=vwstock.itemcode where Minqty>=QOH AND Minqty !=0  ", constr);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2, "vwpurchase");
                dataGridView5.DataSource = ds2.Tables["vwpurchase"].DefaultView;
                dataGridView5.Columns[0].Width = 300;

                SqlDataAdapter da3 = new SqlDataAdapter("select tblItemMaster.ItemName,QOH,Minqty as MOQ from tblItemMaster join vwstock on tblItemMaster.ItemCode=vwstock.itemcode where QOH<=0", constr);
                DataSet ds3 = new DataSet();
                da3.Fill(ds3, "vwpurchase");
                dataGridView8.DataSource = ds3.Tables["vwpurchase"].DefaultView;
                dataGridView8.Columns[0].Width = 300;
                
                SqlDataAdapter da4 = new SqlDataAdapter("select tblItemMaster.ItemName,QOH,tblItemMaster.ROL from tblItemMaster join vwstock on tblItemMaster.ItemCode=vwstock.itemcode where ROL>=QOH and ROL!=0", constr);
                DataSet ds4 = new DataSet();
                da4.Fill(ds4, "vwpurchase");
                dataGridView7.DataSource = ds4.Tables["vwpurchase"].DefaultView;
                dataGridView7.Columns[0].Width = 300;

                
                SqlDataAdapter da5 = new SqlDataAdapter("select  ItemName,sum(QtyOut) as SQty from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblVchType on tblStockHistory.VchType=tblVchType.VchType where tblVchType.ParentID=1 group by CCName,ItemCode,ItemName,UnitName,QtyOut order by SQty DESC", constr);
                DataSet ds5 = new DataSet();
                da5.Fill(ds5, "vwpurchase");
                dataGridView6.DataSource = ds5.Tables["vwpurchase"].DefaultView;
                dataGridView6.Columns[0].Width = 300;
               
                SqlDataAdapter da6 = new SqlDataAdapter("select  ItemName,sum(QtyOut) as SQty from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblVchType on tblStockHistory.VchType=tblVchType.VchType where tblVchType.ParentID=1 group by CCName,ItemCode,ItemName,UnitName,QtyOut order by SQty ASC", constr);
                DataSet ds6 = new DataSet();
                da6.Fill(ds6, "vwpurchase");
                dataGridView9.DataSource = ds6.Tables["vwpurchase"].DefaultView;
                dataGridView9.Columns[0].Width = 300;

                SqlDataAdapter da7 = new SqlDataAdapter("select  ItemName,sum(QtyOut) as SQty from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblVchType on tblStockHistory.VchType=tblVchType.VchType where tblVchType.ParentID=1 group by CCName,ItemCode,ItemName,UnitName,QtyOut order by SQty ASC", constr);
                DataSet ds7 = new DataSet();
                da7.Fill(ds7, "vwpurchase");
                dataGridView10.DataSource = ds7.Tables["vwpurchase"].DefaultView;
                dataGridView10.Columns[0].Width = 300;

            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked==true)
            {
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
            }
        }

       
    }
}
