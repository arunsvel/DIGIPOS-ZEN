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
    public class clsColorMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteColorMaster(UspInsertColorMasterInfo Colorinfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspColorMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@ColorID", SqlDbType.Decimal);
                        SpParam.Value = Colorinfo.ColorID;
                        SpParam = sqlCmd.Parameters.Add("@ColorName", SqlDbType.VarChar);
                        SpParam.Value = Colorinfo.ColorName;
                        SpParam = sqlCmd.Parameters.Add("@ColorHexCode", SqlDbType.VarChar);
                        SpParam.Value = Colorinfo.ColorHexCode;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = Colorinfo.TenantID;
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
        public DataTable GetColorMaster(UspGetColorInfo ColorInfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetColor = new SqlDataAdapter("UspGetColor", sqlcon))
                    {
                        daGetColor.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetColor.SelectCommand.Parameters.Add("@ColorID", SqlDbType.Decimal).Value = ColorInfo.ColorID;
                        daGetColor.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = ColorInfo.TenantID;
                        daGetColor.SelectCommand.Parameters.Add("@ColorIds", SqlDbType.VarChar).Value = ColorInfo.ColorIds;
                        daGetColor.Fill(dtbl);
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
