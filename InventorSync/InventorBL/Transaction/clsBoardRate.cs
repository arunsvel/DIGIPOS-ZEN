using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using InventorSync.JsonClass;
using Newtonsoft.Json;
using System.Collections;

namespace InventorSync.InventorBL.Transaction
{
    public class clsBoardRate : DBConnection
    {
        Common Comm = new Common();

        public string BoardRateMasterCRUD(clsJSonBoardRate clsStockJournal, SqlConnection sqlConn, SqlTransaction trans, string strJson = "", int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            //using (var sqlConn = GetDBConnection())
            //{
            try
            {
                using (SqlCommand sqlCmd = new SqlCommand("UspBoardRateMasterInsert", sqlConn, trans))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter SpParam = new SqlParameter();
                    SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.InvID;
                    SpParam = sqlCmd.Parameters.Add("@InvNo", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.InvNo;
                    SpParam = sqlCmd.Parameters.Add("@InvDate", SqlDbType.DateTime);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.InvDate;
                    SpParam = sqlCmd.Parameters.Add("@VchtypeID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.VchtypeID;
                    SpParam = sqlCmd.Parameters.Add("@Prefix", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.Prefix;
                    SpParam = sqlCmd.Parameters.Add("@SalesManID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMEmployeeInfo_.EmpID;
                    SpParam = sqlCmd.Parameters.Add("@Narration", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.Narration;
                    SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.TenantID;
                    SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Int);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.UserID;
                    SpParam = sqlCmd.Parameters.Add("@JsonData", SqlDbType.VarChar);
                    SpParam.Value = strJson;
                    SpParam = sqlCmd.Parameters.Add("@ReferenceAutoNO", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.ReferenceAutoNO;
                    SpParam = sqlCmd.Parameters.Add("@RefNo", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPLInfo_.RefNo;
                    SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                    SpParam.Value = iAction;

                    SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                    DataSet dsCommon = new DataSet();
                    sqlDa.Fill(dsCommon);
                    dtResult = dsCommon.Tables[0];
                    if (dtResult.Rows.Count > 0)
                        sResult = dtResult.Rows[0]["SqlSpResult"].ToString();

                    if (Convert.ToInt32(sResult) == -1)
                    {
                        sResult = sResult + " | " + dtResult.Rows[0]["ErrorMessage"].ToString();
                        Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                    }
                    return sResult;
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return " - 1" + "| " + ex.Message;
            }
            finally
            {
                //sqlConn.Close();
            }
            //}
        }

        public string BoardRateDetailCRUD(clsJSonBoardRate clsBoardRate, SqlConnection sqlConn, SqlTransaction trans, string sBatchCode = "", int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sBatchCodeNew = "";
            DataTable dbtl = clsBoardRate.clsJsonSJDetailsInfoList_.ToDataTable();
            try
            {
                if (iAction == 0)
                {
                    for (int i = 0; i < dbtl.Rows.Count; i++)
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("UspBoardRateDetailInsert", sqlConn, trans))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                            DataSet dsDtl = new DataSet();
                            SqlParameter SpParam = new SqlParameter();

                            SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                            SpParam.Value = clsBoardRate.clsJsonPLInfo_.InvID; //Convert.ToDecimal(dbtl.Rows[i]["InvID"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@ItemID", SqlDbType.Int);
                            SpParam.Value = Convert.ToInt32(dbtl.Rows[i]["ItemId"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["BatchCode"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["BatchUnique"].ToString();

                            SpParam = sqlCmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["MRP"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@PRate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["PRate"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@CRate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CRate"].ToString());

                            SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate1"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate1Perc", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate1Perc"].ToString());

                            SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate2"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate2Perc", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate2Perc"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate3"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate3Perc", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate3Perc"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate4"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate4Perc", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate4Perc"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate5"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate5Perc", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate5Perc"].ToString());

                            SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                            SpParam.Value = iAction;

                            sqlDa.Fill(dsDtl);

                            dtResult = dsDtl.Tables[0];
                        }
                    }
                }
                else if (iAction == 2)
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspBoardRateDetailInsert", sqlConn, trans))
                    {

                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataSet dsDtl = new DataSet();
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                        SpParam.Value = clsBoardRate.clsJsonPLInfo_.InvID; //Convert.ToDecimal(dbtl.Rows[0]["InvID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ItemID", SqlDbType.Int);
                        SpParam.Value = Convert.ToInt32(dbtl.Rows[0]["ItemId"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchCode"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchUnique"].ToString();

                        SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate1Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1Perc"].ToString());

                        SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate2Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2Perc"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3Perc"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4Perc"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5Perc"].ToString());

                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;

                        sqlDa.Fill(dsDtl);
                        dtResult = dsDtl.Tables[0];
                    }
                }
                else if (iAction == 3)
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspBoardRateDetailInsert", sqlConn, trans))
                    {

                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataSet dsDtl = new DataSet();
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                        SpParam.Value = clsBoardRate.clsJsonPLInfo_.InvID; //Convert.ToDecimal(dbtl.Rows[0]["InvID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ItemID", SqlDbType.Int);
                        SpParam.Value = Convert.ToInt32(dbtl.Rows[0]["ItemId"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchCode"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchUnique"].ToString();

                        SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate1Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1Perc"].ToString());

                        SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate2Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2Perc"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3Perc"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4Perc"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5Perc", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5Perc"].ToString());

                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;

                        sqlDa.Fill(dsDtl);
                        dtResult = dsDtl.Tables[0];
                    }
                }

                string retResult = "";
                if (dtResult.Rows.Count > 0)
                {
                    retResult = dtResult.Rows[0].ItemArray[0].ToString();
                    if (dtResult.Rows[0].ItemArray.Count() > 3)
                    {
                        retResult += "|" + dtResult.Rows[0].ItemArray[4].ToString() + ";" + dtResult.Rows[0].ItemArray[6].ToString() + ";" + dtResult.Rows[0].ItemArray[7].ToString();
                    }
                }
                return retResult;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return "-1" + "| " + ex.Message;
            }
            finally
            {
                //sqlConn.Close();
            }
            //}
        }

        public string PurStockInsert(Hashtable hstStk, string sActon = "")
        {
            string strBatchCode = "";
            strBatchCode = Comm.StockInsert(sActon, Convert.ToDecimal(hstStk["ItemID"].ToString()), hstStk["BatchCode"].ToString(), Convert.ToDecimal(hstStk["Qty"].ToString()), Convert.ToDecimal(hstStk["MRP"].ToString()), Convert.ToDecimal(hstStk["CostRateInc"].ToString()), Convert.ToDecimal(hstStk["CostRateExcl"].ToString()), Convert.ToDecimal(hstStk["PRateExcl"].ToString()), Convert.ToDecimal(hstStk["PrateInc"].ToString()), Convert.ToDecimal(hstStk["TaxPer"].ToString()), Convert.ToDecimal(hstStk["SRate1"].ToString()), Convert.ToDecimal(hstStk["SRate2"].ToString()), Convert.ToDecimal(hstStk["SRate3"].ToString()), Convert.ToDecimal(hstStk["SRate4"].ToString()), Convert.ToDecimal(hstStk["SRate5"].ToString()), Convert.ToInt32(hstStk["BatchMode"].ToString()), hstStk["VchType"].ToString(), Convert.ToDateTime(hstStk["VchDate"].ToString()), Convert.ToDateTime(hstStk["ExpDt"].ToString()), Convert.ToDouble(hstStk["RefID"].ToString()), Convert.ToDouble(hstStk["VchTypeID"].ToString()), Convert.ToDouble(hstStk["CCID"].ToString()), Convert.ToDouble(hstStk["TenantID"].ToString()));
            string[] sData = strBatchCode.Split('|');
            if (sData.Length > 0)
                strBatchCode = sData[0].ToString();

            return strBatchCode;
        }

        public DataTable GetBoardRateMaster(UspGetBoardRateInfo Info, bool blnIsPrevNext = false)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetBoardRateMaster", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@InvId", SqlDbType.Decimal).Value = Info.InvId;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }

        public DataTable GetPriceLisDetailItem(UspGetBoardRateInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetBoardRateDetail", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@InvID", SqlDbType.Decimal).Value = Info.InvId;
                        sqlda.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
    }
}
