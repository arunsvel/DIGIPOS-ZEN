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
    public class clsUserGroup : DBConnection
    {
        Common Comm = new Common();

        public DataTable GetUserGroupMaster(UspGetUserGroupMasterInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetUserGroupMaster", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@GroupID", SqlDbType.VarChar).Value = Info.GroupID;
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
        public string InsertUpdateDeleteUserGroup(Info.UspUserGroupMasterInsertInfo UserGroupMasterInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspUserGroupMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@GroupName", SqlDbType.VarChar);
                        SpParam.Value = UserGroupMasterInsertInfo.GroupName;
                        SpParam = sqlCmd.Parameters.Add("@AccessLevel", SqlDbType.VarChar);
                        SpParam.Value = UserGroupMasterInsertInfo.AccessLevel;
                        SpParam = sqlCmd.Parameters.Add("@StrCCID", SqlDbType.VarChar);
                        SpParam.Value = UserGroupMasterInsertInfo.StrCCID;
                        SpParam = sqlCmd.Parameters.Add("@RptAccesslevel", SqlDbType.VarChar);
                        SpParam.Value = UserGroupMasterInsertInfo.RptAccesslevel;
                        SpParam = sqlCmd.Parameters.Add("@ID", SqlDbType.Decimal);
                        SpParam.Value = UserGroupMasterInsertInfo.ID;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = UserGroupMasterInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = UserGroupMasterInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = UserGroupMasterInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = UserGroupMasterInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = UserGroupMasterInsertInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@BillDisc", SqlDbType.Float);
                        SpParam.Value = UserGroupMasterInsertInfo.BillDisc;
                        SpParam = sqlCmd.Parameters.Add("@ItemDisc", SqlDbType.Float);
                        SpParam.Value = UserGroupMasterInsertInfo.ItemDisc;
                        SpParam = sqlCmd.Parameters.Add("@CashDisc", SqlDbType.Float);
                        SpParam.Value = UserGroupMasterInsertInfo.CashDisc;
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
