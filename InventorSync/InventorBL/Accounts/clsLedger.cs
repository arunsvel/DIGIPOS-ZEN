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
    public class clsLedger : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetLedger(UspGetLedgerInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetLedger", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@LID", SqlDbType.Decimal).Value = Info.LID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@GroupName", SqlDbType.VarChar).Value = Info.GroupName;
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
        public string InsertUpdateDeleteLedger(UspLedgerInsertInfo LedgerInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspLedgerInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@LID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.LID;
                        SpParam = sqlCmd.Parameters.Add("@LName", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.LName;
                        SpParam = sqlCmd.Parameters.Add("@LAliasName", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.LAliasName;
                        SpParam = sqlCmd.Parameters.Add("@GroupName", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.GroupName;
                        SpParam = sqlCmd.Parameters.Add("@Type", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.Type;
                        SpParam = sqlCmd.Parameters.Add("@OpBalance", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.OpBalance;
                        SpParam = sqlCmd.Parameters.Add("@AppearIn", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.AppearIn;
                        SpParam = sqlCmd.Parameters.Add("@Address", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.Address;
                        SpParam = sqlCmd.Parameters.Add("@CreditDays", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.CreditDays;
                        SpParam = sqlCmd.Parameters.Add("@Phone", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.Phone;
                        SpParam = sqlCmd.Parameters.Add("@TaxNo", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.TaxNo;
                        SpParam = sqlCmd.Parameters.Add("@AccountGroupID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.AccountGroupID;
                        SpParam = sqlCmd.Parameters.Add("@RouteID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.RouteID;
                        SpParam = sqlCmd.Parameters.Add("@Area", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.Area;
                        SpParam = sqlCmd.Parameters.Add("@Notes", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.Notes;
                        SpParam = sqlCmd.Parameters.Add("@TargetAmt", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.TargetAmt;
                        SpParam = sqlCmd.Parameters.Add("@SMSSchID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.SMSSchID;
                        SpParam = sqlCmd.Parameters.Add("@Email", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.Email;
                        SpParam = sqlCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.MobileNo;
                        SpParam = sqlCmd.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.DiscPer;
                        SpParam = sqlCmd.Parameters.Add("@InterestPer", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.InterestPer;
                        SpParam = sqlCmd.Parameters.Add("@DummyLName", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.DummyLName;
                        SpParam = sqlCmd.Parameters.Add("@BlnBank", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.BlnBank;
                        SpParam = sqlCmd.Parameters.Add("@CurrencyID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.CurrencyID;
                        SpParam = sqlCmd.Parameters.Add("@AreaID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.AreaID;
                        SpParam = sqlCmd.Parameters.Add("@PLID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.PLID;
                        SpParam = sqlCmd.Parameters.Add("@ActiveStatus", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.ActiveStatus;
                        SpParam = sqlCmd.Parameters.Add("@EmailAddress", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.EmailAddress;
                        SpParam = sqlCmd.Parameters.Add("@EntryDate", SqlDbType.DateTime);
                        SpParam.Value = LedgerInfo.EntryDate;
                        SpParam = sqlCmd.Parameters.Add("@blnBillWise", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.blnBillWise;
                        SpParam = sqlCmd.Parameters.Add("@CustomerCardID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.CustomerCardID;
                        SpParam = sqlCmd.Parameters.Add("@TDSPer", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.TDSPer;
                        SpParam = sqlCmd.Parameters.Add("@DOB", SqlDbType.DateTime);
                        SpParam.Value = LedgerInfo.DOB;
                        SpParam = sqlCmd.Parameters.Add("@StateID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.StateID;
                        SpParam = sqlCmd.Parameters.Add("@CCIDS", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.CCIDS;

                        SpParam = sqlCmd.Parameters.Add("@CurrentBalance", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.CurrentBalance;
                        //SpParam = sqlCmd.Parameters.Add("@LedgerName", SqlDbType.Decimal);
                        //SpParam.Value = LedgerInfo.LedgerName;
                        //SpParam = sqlCmd.Parameters.Add("@LedgerCode", SqlDbType.Decimal);
                        //SpParam.Value = LedgerInfo.LedgerCode;
                        SpParam = sqlCmd.Parameters.Add("@BlnWallet", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.BlnWallet;
                        SpParam = sqlCmd.Parameters.Add("@blnCoupon", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.blnCoupon;
                        SpParam = sqlCmd.Parameters.Add("@TransComn", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.TransComn;
                        SpParam = sqlCmd.Parameters.Add("@BlnSmsWelcome", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.BlnSmsWelcome;
                        SpParam = sqlCmd.Parameters.Add("@DLNO", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.DLNO;
                        SpParam = sqlCmd.Parameters.Add("@TDS", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.TDS;
                        SpParam = sqlCmd.Parameters.Add("@LedgerNameUnicode", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.LedgerNameUnicode;
                        SpParam = sqlCmd.Parameters.Add("@LedgerAliasNameUnicode", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.LedgerNameUnicode;
                        SpParam = sqlCmd.Parameters.Add("@ContactPerson", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.ContactPerson;
                        SpParam = sqlCmd.Parameters.Add("@TaxParameter", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.TaxParameter;
                        SpParam = sqlCmd.Parameters.Add("@TaxParameterType", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.TaxParameterType;
                        SpParam = sqlCmd.Parameters.Add("@HSNCODE", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.HSNCODE;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.CGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.SGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.IGSTTaxPer;
                        //SpParam = sqlCmd.Parameters.Add("@HSNID", SqlDbType.Decimal);
                        //SpParam.Value = LedgerInfo.HSNID;
                        SpParam = sqlCmd.Parameters.Add("@BankAccountNo", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.BankAccountNo;
                        SpParam = sqlCmd.Parameters.Add("@BankNote", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.BankNote;
                        SpParam = sqlCmd.Parameters.Add("@WhatsAppNo", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.WhatsAppNo;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = LedgerInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = LedgerInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@GSTType", SqlDbType.VarChar);
                        SpParam.Value = LedgerInfo.GSTType;
                        SpParam = sqlCmd.Parameters.Add("@AgentID", SqlDbType.Decimal);
                        SpParam.Value = LedgerInfo.AgentID;

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
        public DataTable GetLedgerDetail(UspGetLedgerInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetLedgerDetail", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@LID", SqlDbType.Decimal).Value = Info.LID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@AccGpID", SqlDbType.Decimal).Value = Info.AccGroupID;
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
