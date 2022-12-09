using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;

namespace DigiposZen.InventorBL.Accounts
{
    public class clsAccountGroup : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetAccountGroup(UspGetAccountGroupInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetAccountGroup", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@AccountGroupID", SqlDbType.Decimal).Value = Info.AccountGroupID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
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
        public string InsertUpdateDeleteAccountGroup(UspAccountGroupInsertInfo AccGroupInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspAccountGroupInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@AccountGroupID", SqlDbType.Decimal);
                        SpParam.Value = AccGroupInfo.AccountGroupID;
                        SpParam = sqlCmd.Parameters.Add("@AccountGroup", SqlDbType.VarChar);
                        SpParam.Value = AccGroupInfo.AccountGroup;
                        SpParam = sqlCmd.Parameters.Add("@Nature", SqlDbType.VarChar);
                        SpParam.Value = AccGroupInfo.Nature;
                        SpParam = sqlCmd.Parameters.Add("@MaintainBudget", SqlDbType.Decimal);
                        SpParam.Value = AccGroupInfo.MaintainBudget;
                        SpParam = sqlCmd.Parameters.Add("@SortOrder", SqlDbType.Decimal);
                        SpParam.Value = AccGroupInfo.SortOrder;
                        SpParam = sqlCmd.Parameters.Add("@ParentID", SqlDbType.Decimal);
                        SpParam.Value = AccGroupInfo.ParentID;
                        SpParam = sqlCmd.Parameters.Add("@HID", SqlDbType.VarChar);
                        SpParam.Value = AccGroupInfo.HID;
                        SpParam = sqlCmd.Parameters.Add("@ACTIVESTATUS", SqlDbType.Int);
                        SpParam.Value = AccGroupInfo.ACTIVESTATUS;

                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = AccGroupInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = AccGroupInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = AccGroupInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = AccGroupInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = AccGroupInfo.TenantID;

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
                        else
                        {
                            sResult = "";
                            foreach (DataColumn cl in dtResult.Columns)
                            {
                                sResult = sResult + "|" + dtResult.Rows[0][cl].ToString();
                            }
                            sResult = sResult.Substring(1, sResult.Length - 1);
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
        public DataTable CheckParentIDExists(UspGetAccountGroupInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daArea = new SqlDataAdapter("UspGetAccountGroupParentid", sqlcon))
                    {
                        daArea.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daArea.SelectCommand.Parameters.Add("@AccountGroupID", SqlDbType.Int).Value = Info.AccountGroupID;
                        daArea.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
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
    }
}
