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
    class clsAgentMaster:DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteAgentMaster(UspAgentMasterInfo AgentInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspAgentInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@AgentID", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.AgentID;
                        SpParam = sqlCmd.Parameters.Add("@AgentCode", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.AgentCode;
                        SpParam = sqlCmd.Parameters.Add("@AgentName", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.AgentName;
                        SpParam = sqlCmd.Parameters.Add("@Area", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.Area;
                        SpParam = sqlCmd.Parameters.Add("@Commission", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.Commission;                      
                        SpParam = sqlCmd.Parameters.Add("@blnPOstAccounts", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.blnPOstAccounts;
                        SpParam = sqlCmd.Parameters.Add("@ADDRESS", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.ADDRESS;
                        SpParam = sqlCmd.Parameters.Add("@LOCATION", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.LOCATION;
                        SpParam = sqlCmd.Parameters.Add("@PHONE", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.PHONE;
                        SpParam = sqlCmd.Parameters.Add("@WEBSITE", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.WEBSITE;
                        SpParam = sqlCmd.Parameters.Add("@EMAIL", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.EMAIL;                                            
                        SpParam = sqlCmd.Parameters.Add("@BLNROOMRENT", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.BLNROOMRENT;
                        SpParam = sqlCmd.Parameters.Add("@BLNSERVICES", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.BLNSERVICES;
                        SpParam = sqlCmd.Parameters.Add("@blnItemwiseCommission", SqlDbType.Int);
                        SpParam.Value = AgentInfo.blnItemwiseCommission;
                        SpParam = sqlCmd.Parameters.Add("@AgentDiscount", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.AgentDiscount;                      
                        SpParam = sqlCmd.Parameters.Add("@LID", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.LID;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = AgentInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = AgentInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = AgentInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@AreaID", SqlDbType.Decimal);
                        SpParam.Value = AgentInfo.AreaID;
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
        public DataTable GetAgentMaster(UspGetAgentinfo GetAgentinfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetAgent = new SqlDataAdapter("UspGetAgent", sqlcon))
                    {
                        daGetAgent.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetAgent.SelectCommand.Parameters.Add("@AgentID", SqlDbType.Decimal).Value = GetAgentinfo.AgentID;
                        daGetAgent.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = GetAgentinfo.TenantID;
                        daGetAgent.Fill(dtbl);
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
