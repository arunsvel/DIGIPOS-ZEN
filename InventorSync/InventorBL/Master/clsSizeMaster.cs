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
    public class clsSizeMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteSizeMaster(UspInsertSizeMasterInfo sizeinfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspSizeMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@SizeID", SqlDbType.Decimal);
                        SpParam.Value = sizeinfo.SizeID;
                        SpParam = sqlCmd.Parameters.Add("@SizeName", SqlDbType.VarChar);
                        SpParam.Value = sizeinfo.SizeName;
                        SpParam = sqlCmd.Parameters.Add("@SizeNameShort", SqlDbType.VarChar);
                        SpParam.Value = sizeinfo.SizeNameShort;
                        SpParam = sqlCmd.Parameters.Add("@SortOrder", SqlDbType.Decimal);
                        SpParam.Value = sizeinfo.SortOrder;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = sizeinfo.TenantID;
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
        public DataTable GetSizeMaster(UspGetSizeInfo SizeInfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetSize = new SqlDataAdapter("UspGetSize", sqlcon))
                    {
                        daGetSize.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetSize.SelectCommand.Parameters.Add("@SizeID", SqlDbType.Decimal).Value = SizeInfo.SizeID;
                        daGetSize.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = SizeInfo.TenantID;
                        daGetSize.SelectCommand.Parameters.Add("@SizeIds", SqlDbType.VarChar).Value = SizeInfo.SizeIds;
                        daGetSize.Fill(dtbl);
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
