using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DigiposZen.InventorBL.Helper;

namespace DigiposZen.InventorBL.Master
{
    public class clsManufacturer : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteManufacturer(Info.UspManufacturerInsertInfo ManfactureInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspManufacturerInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@MnfID", SqlDbType.Decimal);
                        SpParam.Value = ManfactureInfo.MnfID;
                        SpParam = sqlCmd.Parameters.Add("@MnfName", SqlDbType.VarChar);
                        SpParam.Value = ManfactureInfo.MnfName;
                        SpParam = sqlCmd.Parameters.Add("@MnfShortName", SqlDbType.VarChar);
                        SpParam.Value = ManfactureInfo.MnfShortName;
                        SpParam = sqlCmd.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                        SpParam.Value = ManfactureInfo.DiscPer;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = ManfactureInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = ManfactureInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = ManfactureInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = ManfactureInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = ManfactureInfo.LastUpdateTime;
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
        public DataTable GetManufacturer(Info.UspGetManufacturerInfo GetManf)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daManf = new SqlDataAdapter("UspGetManufacturer", sqlcon))
                    {
                        daManf.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daManf.SelectCommand.Parameters.Add("@MnfID", SqlDbType.Decimal).Value = GetManf.MnfID;
                        daManf.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = GetManf.TenantID;
                        daManf.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
        public DataTable GetManufacturerForItemMaster(Info.UspGetManufacturerForItemMasterInfo GetManf)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daManf = new SqlDataAdapter("UspGetManufacturerForItemMaster", sqlcon))
                    {
                        daManf.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daManf.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Decimal).Value = GetManf.ItemID;
                        daManf.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = GetManf.TenantID;
                        daManf.Fill(dtbl);
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
