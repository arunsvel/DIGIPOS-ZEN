using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;

namespace DigiposZen.InventorBL.Master
{
    public class clsTaxMode : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetTaxMode(UspGetTaxModeInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetTaxMode", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@TaxModeID", SqlDbType.Decimal).Value = Info.TaxModeID;
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
        public string InsertUpdateDeleteTaxMode(UspTaxModeInsertInfo Taxinfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspTaxModeInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@TaxModeID", SqlDbType.Decimal);
                        SpParam.Value = Taxinfo.TaxModeID;
                        SpParam = sqlCmd.Parameters.Add("@TaxMode", SqlDbType.VarChar);
                        SpParam.Value = Taxinfo.TaxMode;
                        SpParam = sqlCmd.Parameters.Add("@CalculationID", SqlDbType.Decimal);
                        SpParam.Value = Taxinfo.CalculationID;
                        SpParam = sqlCmd.Parameters.Add("@SortNo", SqlDbType.Decimal);
                        SpParam.Value = Taxinfo.SortNo;
                        SpParam = sqlCmd.Parameters.Add("@ActiveStatus", SqlDbType.Decimal);
                        SpParam.Value = Taxinfo.ActiveStatus;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = Taxinfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = Taxinfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = Taxinfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = Taxinfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = Taxinfo.TenantID;
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
                            sResult = sResult + "|" + dtResult.Rows[0]["ErrorMessage"].ToString();
                            Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                        }
                        else
                        {
                            sResult = "";
                            foreach (DataColumn cl in dtResult.Columns)
                            {
                                sResult = sResult + "|" + dtResult.Rows[0][cl].ToString();
                            }
                            sResult = sResult.Substring(1, sResult.Length - 1);
                        }

                        return sResult;
                    }
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return "-1" + "|" + ex.Message;
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }

        public DataTable GetTaxModeLastUpdateDetails(UspGetTaxModeInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetTaxModeLastUpdateDetails", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
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
    }
}
