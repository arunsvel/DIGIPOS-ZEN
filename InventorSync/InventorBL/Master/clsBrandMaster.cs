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
    public class clsBrandMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteBrandMaster(UspInsertBrandMasterInfo BrandInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspBrandMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@brandID", SqlDbType.Decimal);
                        SpParam.Value = BrandInfo.brandID;
                        SpParam = sqlCmd.Parameters.Add("@brandName", SqlDbType.VarChar);
                        SpParam.Value = BrandInfo.brandName;
                        SpParam = sqlCmd.Parameters.Add("@brandShortName", SqlDbType.VarChar);
                        SpParam.Value = BrandInfo.brandShortName;
                        SpParam = sqlCmd.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                        SpParam.Value = BrandInfo.DiscPer;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = BrandInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = BrandInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = BrandInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = BrandInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = BrandInfo.LastUpdateTime;
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
        public DataTable GetBrandMaster(UspGetBrandinfo Brandinfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daBRand = new SqlDataAdapter("UspGetBrand", sqlcon))
                    {
                        daBRand.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daBRand.SelectCommand.Parameters.Add("@brandID", SqlDbType.Decimal).Value = Brandinfo.brandID;
                        daBRand.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = Brandinfo.TenantID;
                        daBRand.Fill(dtbl);
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
