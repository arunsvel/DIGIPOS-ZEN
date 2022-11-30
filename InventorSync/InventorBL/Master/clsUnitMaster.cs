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
    class clsUnitMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteUnitMaster(UspInsertUnitMasterInfo  UnitInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspUnitInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@UnitID", SqlDbType.Decimal);
                        SpParam.Value = UnitInfo.UnitID;
                        SpParam = sqlCmd.Parameters.Add("@UnitName", SqlDbType.VarChar);
                        SpParam.Value = UnitInfo.UnitName;
                        SpParam = sqlCmd.Parameters.Add("@UnitShortName", SqlDbType.VarChar);
                        SpParam.Value = UnitInfo.UnitShortName;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = UnitInfo.TenantID;
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
        public DataTable GetUnitMaster(UspGetUnitInfo UnitInfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetUnit = new SqlDataAdapter("UspGetUnit", sqlcon))
                    {
                        daGetUnit.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetUnit.SelectCommand.Parameters.Add("@UnitID", SqlDbType.Decimal).Value = UnitInfo.UnitID;
                        daGetUnit.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = UnitInfo.TenantID;
                        daGetUnit.Fill(dtbl);
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
