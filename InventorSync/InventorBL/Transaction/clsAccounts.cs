using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using InventorSync.JsonClass;
using Newtonsoft.Json;
using System.Collections;

namespace InventorSync.InventorBL.Transaction
{
    public class clsAccounts : DBConnection
    {
        Common Comm = new Common();

        public string InsertUpdateDeleteAccVoucherInsert(clsJsonACCInfo AccVoucherInsertInfo, SqlConnection sqlConn, SqlTransaction trans, string strJson = "", int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            //using (var sqlConn = GetDBConnection())
            //{
            try
            {
                using (SqlCommand sqlCmd = new SqlCommand("UspAccVoucherInsert", sqlConn, trans))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter SpParam = new SqlParameter();
                    SpParam = sqlCmd.Parameters.Add("@InvId", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.InvId;
                    SpParam = sqlCmd.Parameters.Add("@InvNo", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.InvNo;
                    SpParam = sqlCmd.Parameters.Add("@AutoNum", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.AutoNum;
                    SpParam = sqlCmd.Parameters.Add("@ReferenceAutoNO", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.ReferenceAutoNO;
                    SpParam = sqlCmd.Parameters.Add("@Prefix", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.Prefix;
                    SpParam = sqlCmd.Parameters.Add("@InvDate", SqlDbType.DateTime);
                    SpParam.Value = AccVoucherInsertInfo.InvDate;
                    SpParam = sqlCmd.Parameters.Add("@VchType", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.VchType;
                    SpParam = sqlCmd.Parameters.Add("@DebitCredit", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.DebitCredit;
                    SpParam = sqlCmd.Parameters.Add("@ACCLedgerID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.ACCLedgerID;
                    SpParam = sqlCmd.Parameters.Add("@TaxModeID", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.TaxModeID;
                    SpParam = sqlCmd.Parameters.Add("@LedgerId", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.LedgerId;
                    SpParam = sqlCmd.Parameters.Add("@Party", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.Party;
                    SpParam = sqlCmd.Parameters.Add("@TaxAmt", SqlDbType.Float);
                    SpParam.Value = AccVoucherInsertInfo.TaxAmt;
                    SpParam = sqlCmd.Parameters.Add("@GrossAmt", SqlDbType.Float);
                    SpParam.Value = AccVoucherInsertInfo.GrossAmt;
                    SpParam = sqlCmd.Parameters.Add("@BillAmt", SqlDbType.Float);
                    SpParam.Value = AccVoucherInsertInfo.BillAmt;
                    SpParam = sqlCmd.Parameters.Add("@Cancelled", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.Cancelled;
                    SpParam = sqlCmd.Parameters.Add("@SalesManID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.SalesManID;
                    SpParam = sqlCmd.Parameters.Add("@Taxable", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.Taxable;
                    SpParam = sqlCmd.Parameters.Add("@NonTaxable", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.NonTaxable;
                    SpParam = sqlCmd.Parameters.Add("@UserNarration", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.UserNarration;
                    SpParam = sqlCmd.Parameters.Add("@SortNumber", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.SortNumber;
                    SpParam = sqlCmd.Parameters.Add("@VchTypeID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.VchTypeID;
                    SpParam = sqlCmd.Parameters.Add("@CCID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.CCID;
                    SpParam = sqlCmd.Parameters.Add("@CurrencyID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.CurrencyID;
                    SpParam = sqlCmd.Parameters.Add("@PartyAddress", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.PartyAddress;
                    SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Int);
                    SpParam.Value = AccVoucherInsertInfo.UserID;
                    SpParam = sqlCmd.Parameters.Add("@CashDiscount", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.CashDiscount;
                    SpParam = sqlCmd.Parameters.Add("@NetAmount", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.NetAmount;
                    SpParam = sqlCmd.Parameters.Add("@RefNo", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.RefNo;
                    SpParam = sqlCmd.Parameters.Add("@blnWaitforAuthorisation", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.blnWaitforAuthorisation;
                    SpParam = sqlCmd.Parameters.Add("@UserIDAuth", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.UserIDAuth;
                    SpParam = sqlCmd.Parameters.Add("@BillTime", SqlDbType.DateTime);
                    SpParam.Value = AccVoucherInsertInfo.BillTime;
                    SpParam = sqlCmd.Parameters.Add("@StateID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.StateID;
                    SpParam = sqlCmd.Parameters.Add("@ImplementingStateCode", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.ImplementingStateCode;
                    SpParam = sqlCmd.Parameters.Add("@GSTType", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.GSTType;
                    SpParam = sqlCmd.Parameters.Add("@CGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.CGSTTotal;
                    SpParam = sqlCmd.Parameters.Add("@SGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.SGSTTotal;
                    SpParam = sqlCmd.Parameters.Add("@IGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.IGSTTotal;
                    SpParam = sqlCmd.Parameters.Add("@PartyGSTIN", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.PartyGSTIN;
                    SpParam = sqlCmd.Parameters.Add("@BillType", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.BillType;
                    SpParam = sqlCmd.Parameters.Add("@blnHold", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.blnHold;
                    SpParam = sqlCmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTime);
                    SpParam.Value = AccVoucherInsertInfo.EffectiveDate;
                    SpParam = sqlCmd.Parameters.Add("@partyCode", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.partyCode;
                    SpParam = sqlCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.MobileNo;
                    SpParam = sqlCmd.Parameters.Add("@Email", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.Email;
                    SpParam = sqlCmd.Parameters.Add("@TaxType", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.TaxType;
                    SpParam = sqlCmd.Parameters.Add("@QtyTotal", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.QtyTotal;
                    SpParam = sqlCmd.Parameters.Add("@REconciled", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.REconciled;
                    SpParam = sqlCmd.Parameters.Add("@Status", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.Status;
                    SpParam = sqlCmd.Parameters.Add("@ChequeNo", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.ChequeNo;
                    SpParam = sqlCmd.Parameters.Add("@BankName", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.BankName;
                    SpParam = sqlCmd.Parameters.Add("@SalesTaxtype", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.SalesTaxtype;
                    SpParam = sqlCmd.Parameters.Add("@TransType", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.TransType;
                    SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.SystemName;
                    SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                    SpParam.Value = AccVoucherInsertInfo.LastUpdateDate;
                    SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                    SpParam.Value = AccVoucherInsertInfo.LastUpdateTime;
                    SpParam = sqlCmd.Parameters.Add("@CounterID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.CounterID;
                    SpParam = sqlCmd.Parameters.Add("@AdavancedMode", SqlDbType.VarChar);
                    SpParam.Value = AccVoucherInsertInfo.AdavancedMode;
                    SpParam = sqlCmd.Parameters.Add("@ChequeDate", SqlDbType.DateTime);
                    SpParam.Value = AccVoucherInsertInfo.ChequeDate;
                    SpParam = sqlCmd.Parameters.Add("@JsonData", SqlDbType.VarChar);
                    SpParam.Value = strJson;
                    SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                    SpParam.Value = AccVoucherInsertInfo.TenantID;
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
                        sResult = sResult + " | " + dtResult.Rows[0]["ErrorMessage"].ToString();
                        Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                    }
                    return sResult;
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return "-1" + "| " + ex.Message;
            }
            finally
            {
                //sqlConn.Close();
            }
            //}
        }

        public string InsertUpdateDeleteAccVoucherItemInsert(clsJSonAccounts AccVoucherItemInsertInfo, SqlConnection sqlConn, SqlTransaction trans, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";

            DataTable dbtl = AccVoucherItemInsertInfo.clsJsonACCDetailsInfoList_.ToDataTable();

            try
            {
                for (int i = 0; i < dbtl.Rows.Count; i++)
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspAccVoucherItemInsert", sqlConn, trans))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["InvID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@LID", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["LID"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Qty"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@Amount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Amount"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@AmountD", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["AmountD"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@AmountC", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["AmountC"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@TaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["TaxPer"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["TaxAmount"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@SlNo", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["SlNo"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@ITaxableAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ITaxableAmount"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@INetAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["INetAmount"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CGSTTaxPer"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CGSTTaxAmt"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["SGSTTaxPer"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["SGSTTaxAmt"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IGSTTaxPer"].ToString()); //;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IGSTTaxAmt"].ToString()); //;
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
                            sResult = sResult + " | " + dtResult.Rows[0]["ErrorMessage"].ToString();
                            Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                        }
                    }
                }

                string retResult = "";
                if (dtResult.Rows.Count > 0)
                {
                    retResult = dtResult.Rows[0].ItemArray[0].ToString();
                    if (dtResult.Rows[0].ItemArray.Count() > 3)
                    {
                        retResult += "|" + dtResult.Rows[0].ItemArray[4].ToString() + ";" + dtResult.Rows[0].ItemArray[6].ToString() + ";" + dtResult.Rows[0].ItemArray[7].ToString();
                    }
                }
                return retResult;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return "-1" + "| " + ex.Message;
            }
            finally
            {
                //sqlConn.Close();
            }
            //}
        }

        public DataTable GetAccountsMaster(UspGetAccountsInfo Info, bool blnIsPrevNext = false)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetAccountsMaster", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@InvId", SqlDbType.Decimal).Value = Info.InvId;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@VchTypeID", SqlDbType.Decimal).Value = Info.VchTypeID;
                        sqlda.SelectCommand.Parameters.Add("@blnPrevNext", SqlDbType.Bit).Value = blnIsPrevNext;
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

        public DataTable GetAccountsDetailItem(UspGetAccountsInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetAccountsDetailItem", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@InvID", SqlDbType.Decimal).Value = Info.InvId;
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
