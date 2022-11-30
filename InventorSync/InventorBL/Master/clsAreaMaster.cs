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
    class clsAreaMaster:DBConnection
    {
        #region "Insert|Update|Delete------------------------------------------ >> "
        Common Comm = new Common();
        public string InsertUpdateDeleteAreaMaster(UspAreaMasterInfo AreaInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspAreaMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@AreaID", SqlDbType.Decimal);
                        SpParam.Value = AreaInfo.AreaID;
                        SpParam = sqlCmd.Parameters.Add("@Area", SqlDbType.VarChar);
                        SpParam.Value = AreaInfo.Area;
                        SpParam = sqlCmd.Parameters.Add("@Remarks", SqlDbType.VarChar);
                        SpParam.Value =AreaInfo.Remarks;
                        SpParam = sqlCmd.Parameters.Add("@ParentID", SqlDbType.VarChar);
                        SpParam.Value = AreaInfo.ParentID;
                        SpParam = sqlCmd.Parameters.Add("@HID", SqlDbType.VarChar);
                        SpParam.Value = AreaInfo.HID;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = AreaInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = AreaInfo.UserID;                       
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = AreaInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = AreaInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = AreaInfo.TenantID;
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
        public DataTable GetAreaMaster(UspAreaMasterInfo getAreaInfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetArea = new SqlDataAdapter("UspGetArea", sqlcon))
                    {
                        daGetArea.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetArea.SelectCommand.Parameters.Add("@AreaID", SqlDbType.Decimal).Value = getAreaInfo.AreaID;
                        daGetArea.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = getAreaInfo.TenantID;
                        daGetArea.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
        public DataTable CheckParentIDExists(decimal parentID, decimal TenantID)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daArea = new SqlDataAdapter("UspGetAreaParentid", sqlcon))
                    {
                        daArea.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daArea.SelectCommand.Parameters.Add("@AreID", SqlDbType.Int).Value = Convert.ToInt32(parentID);
                        daArea.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = TenantID;
                        daArea.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
        #endregion
    }

}

