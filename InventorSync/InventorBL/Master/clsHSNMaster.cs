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
    public class clsHSNMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteHSNMaster(UspInsertHSNmasterInfo HSNmasterInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspHSNMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@HID", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.HID;
                        SpParam = sqlCmd.Parameters.Add("@HSNCODE", SqlDbType.VarChar);
                        SpParam.Value = HSNmasterInfo.HSNCODE;
                        SpParam = sqlCmd.Parameters.Add("@HSNDECRIPTION", SqlDbType.VarChar);
                        SpParam.Value = HSNmasterInfo.HSNDECRIPTION;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.VarChar);
                        SpParam.Value = HSNmasterInfo.CGSTTaxPer;

                        SpParam = sqlCmd.Parameters.Add("@blnSlabSystem", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.blnSlabSystem;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer1", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.CGSTTaxPer1;

                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer2", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.CGSTTaxPer2;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer3", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.CGSTTaxPer3;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer4", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.CGSTTaxPer4;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.SGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer1", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.SGSTTaxPer1;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer2", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.SGSTTaxPer2;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer3", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.SGSTTaxPer3;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer4", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.SGSTTaxPer4;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.IGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer1", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.IGSTTaxPer1;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer2", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.IGSTTaxPer2;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer3", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.IGSTTaxPer3;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer4", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.IGSTTaxPer4;
                        SpParam = sqlCmd.Parameters.Add("@ValueStartSB1", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueStartSB1;
                        SpParam = sqlCmd.Parameters.Add("@ValueStartSB2", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueStartSB2;
                        SpParam = sqlCmd.Parameters.Add("@ValueStartSB3", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueStartSB3;
                        SpParam = sqlCmd.Parameters.Add("@ValueStartSB4", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueStartSB4;
                        SpParam = sqlCmd.Parameters.Add("@ValueEndSB1", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueEndSB1;
                        SpParam = sqlCmd.Parameters.Add("@ValueEndSB2", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueEndSB2;
                        SpParam = sqlCmd.Parameters.Add("@ValueEndSB3", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueEndSB3;
                        SpParam = sqlCmd.Parameters.Add("@ValueEndSB4", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.ValueEndSB4;
                        SpParam = sqlCmd.Parameters.Add("@CessPer", SqlDbType.VarChar);
                        SpParam.Value = HSNmasterInfo.CessPer;
                        SpParam = sqlCmd.Parameters.Add("@CompCessQty", SqlDbType.VarChar);
                        SpParam.Value = HSNmasterInfo.CompCessQty;
                        SpParam = sqlCmd.Parameters.Add("@HSNType", SqlDbType.VarChar);
                        SpParam.Value = HSNmasterInfo.HSNType;



                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = HSNmasterInfo.TenantID;
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
        public DataTable GetHSNMaster(UspGetHSNInfo HSNInfo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetHSN= new SqlDataAdapter("UspGetHSN", sqlcon))
                    {
                        daGetHSN.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetHSN.SelectCommand.Parameters.Add("@HID", SqlDbType.Decimal).Value = HSNInfo.HID;
                        daGetHSN.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = HSNInfo.TenantID;
                        daGetHSN.Fill(dtbl);
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
