using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;

namespace InventorSync.InventorBL.Master
{
    public class clsState : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetStates(UspGetStateInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStates", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@StateId", SqlDbType.Decimal).Value = Info.StateId;
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
        public string InsertUpdateDeleteStates(Info.UspInsertStateInfo StatesInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspStatesInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@StateId", SqlDbType.Decimal);
                        SpParam.Value = StatesInsertInfo.StateId;
                        SpParam = sqlCmd.Parameters.Add("@StateCode", SqlDbType.VarChar);
                        SpParam.Value = StatesInsertInfo.StateCode;
                        SpParam = sqlCmd.Parameters.Add("@State", SqlDbType.VarChar);
                        SpParam.Value = StatesInsertInfo.State;
                        SpParam = sqlCmd.Parameters.Add("@StateType", SqlDbType.VarChar);
                        SpParam.Value = StatesInsertInfo.StateType;
                        SpParam = sqlCmd.Parameters.Add("@Country", SqlDbType.VarChar);
                        SpParam.Value = StatesInsertInfo.Country;
                        SpParam = sqlCmd.Parameters.Add("@CountryID", SqlDbType.Decimal);
                        SpParam.Value = StatesInsertInfo.CountryID;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = StatesInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = StatesInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = StatesInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = StatesInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = StatesInsertInfo.TenantID;
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
                    sqlConn.Close();
                }
            }
        }

    }
}
