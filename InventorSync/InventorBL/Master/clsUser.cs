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
    public class clsUser : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteUserMaster(UspUserMasterInsertInfo UserMasterInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspUserMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Int);
                        SpParam.Value = UserMasterInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@UserName", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.UserName;
                        SpParam = sqlCmd.Parameters.Add("@Pwd", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.Pwd;
                        SpParam = sqlCmd.Parameters.Add("@GroupID", SqlDbType.Int);
                        SpParam.Value = UserMasterInsertInfo.GroupID;
                        SpParam = sqlCmd.Parameters.Add("@Status", SqlDbType.Int);
                        SpParam.Value = UserMasterInsertInfo.Status;
                        SpParam = sqlCmd.Parameters.Add("@changepwdonlogon", SqlDbType.Decimal);
                        SpParam.Value = UserMasterInsertInfo.changepwdonlogon;
                        SpParam = sqlCmd.Parameters.Add("@CostCentre", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.CostCentre;
                        SpParam = sqlCmd.Parameters.Add("@HintAnswer", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.HintAnswer;
                        SpParam = sqlCmd.Parameters.Add("@HintQuestion", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.HintQuestion;
                        SpParam = sqlCmd.Parameters.Add("@WorkingDays", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.WorkingDays;
                        SpParam = sqlCmd.Parameters.Add("@WorkFrom", SqlDbType.DateTime);
                        SpParam.Value = UserMasterInsertInfo.WorkFrom;
                        SpParam = sqlCmd.Parameters.Add("@WorkTo", SqlDbType.DateTime);
                        SpParam.Value = UserMasterInsertInfo.WorkTo;
                        SpParam = sqlCmd.Parameters.Add("@godown", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.godown;
                        SpParam = sqlCmd.Parameters.Add("@SelectedCCID", SqlDbType.Decimal);
                        SpParam.Value = UserMasterInsertInfo.SelectedCCID;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@ID", SqlDbType.Decimal);
                        SpParam.Value = UserMasterInsertInfo.ID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = UserMasterInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = UserMasterInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@OrderVchtypeIDs", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.OrderVchtypeIDs;
                        SpParam = sqlCmd.Parameters.Add("@SalesVchtypeIDs", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.SalesVchtypeIDs;
                        SpParam = sqlCmd.Parameters.Add("@SalesReturnVchtypeIDs", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.SalesReturnVchtypeIDs;
                        SpParam = sqlCmd.Parameters.Add("@AccountsVchtypeIDs", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.AccountsVchtypeIDs;
                        SpParam = sqlCmd.Parameters.Add("@UserLedgerID", SqlDbType.Decimal);
                        SpParam.Value = UserMasterInsertInfo.UserLedgerID;
                        SpParam = sqlCmd.Parameters.Add("@ActiveCounterID", SqlDbType.Decimal);
                        SpParam.Value = UserMasterInsertInfo.ActiveCounterID;
                        SpParam = sqlCmd.Parameters.Add("@PIN", SqlDbType.Decimal);
                        SpParam.Value = UserMasterInsertInfo.PIN;
                        SpParam = sqlCmd.Parameters.Add("@CCIDS", SqlDbType.VarChar);
                        SpParam.Value = UserMasterInsertInfo.CCIDs;
                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;
                        SpParam = sqlCmd.Parameters.Add("@StartupUserID", SqlDbType.Int);
                        SpParam.Value = UserMasterInsertInfo.StartupUserID;
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
        public DataTable GetUserMaster(UspGetUserMasterInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetUserMaster", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = Info.UserID;
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
    }
}
