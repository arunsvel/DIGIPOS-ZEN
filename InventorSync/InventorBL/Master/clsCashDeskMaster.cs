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
    public class clsCashDeskMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteCashDeskMaster(UspInsertCashDeskMaster CashDeskinfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspCashDeskMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@PaymentID", SqlDbType.Decimal);
                        SpParam.Value = CashDeskinfo.PaymentID;
                        SpParam = sqlCmd.Parameters.Add("@PaymentType", SqlDbType.VarChar);
                        SpParam.Value = CashDeskinfo.PaymentType;
                        SpParam = sqlCmd.Parameters.Add("LedgerID", SqlDbType.Decimal);
                        SpParam.Value = CashDeskinfo.LedgerID;
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
        public DataTable GetCashDeskMaster(UspGetCashDeskIMasterInfo CashDeskMaster)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daGetColor = new SqlDataAdapter("UspGetCashDeskMaster", sqlcon))
                    {
                        daGetColor.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daGetColor.SelectCommand.Parameters.Add("@PaymentID", SqlDbType.Decimal).Value = CashDeskMaster.PaymentID;
                        daGetColor.SelectCommand.Parameters.Add("@PaymentIDs", SqlDbType.VarChar).Value = CashDeskMaster.Paymentids;
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
