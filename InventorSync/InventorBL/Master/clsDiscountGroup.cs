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
    public class clsDiscountGroup : DBConnection
    {
        Common Comm = new Common();
        public string  InsertUpdateDeleteDiscountGroup(Info.UspInsertDiscountGroupInfo DiscountGroupInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspDiscountGroupInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@DiscountGroupID", SqlDbType.Decimal);
                        SpParam.Value = DiscountGroupInfo.DiscountGroupID;
                        SpParam = sqlCmd.Parameters.Add("@DiscountGroupName", SqlDbType.VarChar);
                        SpParam.Value = DiscountGroupInfo.DiscountGroupName;
                        SpParam = sqlCmd.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                        SpParam.Value = DiscountGroupInfo.DiscPer;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = DiscountGroupInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = DiscountGroupInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = DiscountGroupInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = DiscountGroupInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = DiscountGroupInfo.LastUpdateTime;
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
        public DataTable GetDiscountGroup(UspGetDiscountGroupInfo DiscGInfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetDiscG = new SqlDataAdapter("UspGetDiscountGroup", sqlcon))
                    {
                        daGetDiscG.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetDiscG.SelectCommand.Parameters.Add("@DiscountGroupID", SqlDbType.Decimal).Value = DiscGInfo.DiscountGroupID;
                        daGetDiscG.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = DiscGInfo.TenantID;
                        daGetDiscG.Fill(dtbl);
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