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
    public class clsStockJournal : DBConnection
    {
        Common Comm = new Common();

        public string StockJournalMasterCRUD(clsJSonStockJournal clsStockJournal, SqlConnection sqlConn, SqlTransaction trans, string strJson = "", int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            //using (var sqlConn = GetDBConnection())
            //{
            try
            {
                using (SqlCommand sqlCmd = new SqlCommand("UspStockJournalInsert", sqlConn, trans))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter SpParam = new SqlParameter();
                    SpParam = sqlCmd.Parameters.Add("@InvId", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.InvId;
                    SpParam = sqlCmd.Parameters.Add("@InvNo", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.InvNo;
                    SpParam = sqlCmd.Parameters.Add("@AutoNum", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.AutoNum;
                    SpParam = sqlCmd.Parameters.Add("@Prefix", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.Prefix;
                    SpParam = sqlCmd.Parameters.Add("@InvDate", SqlDbType.DateTime);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.InvDate;
                    SpParam = sqlCmd.Parameters.Add("@VchType", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.VchType;
                    SpParam = sqlCmd.Parameters.Add("@MOP", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.MOP;
                    SpParam = sqlCmd.Parameters.Add("@TaxModeID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.TaxModeID;
                    SpParam = sqlCmd.Parameters.Add("@LedgerId", SqlDbType.Decimal);
                    SpParam.Value = 0;
                    SpParam = sqlCmd.Parameters.Add("@Party", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.Discount;
                    SpParam = sqlCmd.Parameters.Add("@TaxAmt", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.TaxAmt;
                    SpParam = sqlCmd.Parameters.Add("@GrossAmt", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.GrossAmt;
                    SpParam = sqlCmd.Parameters.Add("@BillAmt", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.BillAmt;
                    SpParam = sqlCmd.Parameters.Add("@Cancelled", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.Cancelled;
                    SpParam = sqlCmd.Parameters.Add("@OtherExpense", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.OtherExpense;
                    SpParam = sqlCmd.Parameters.Add("@SalesManID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMEmployeeInfo_.EmpID;
                    SpParam = sqlCmd.Parameters.Add("@Taxable", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.Taxable;
                    SpParam = sqlCmd.Parameters.Add("@NonTaxable", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.NonTaxable;
                    SpParam = sqlCmd.Parameters.Add("@ItemDiscountTotal", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ItemDiscountTotal;
                    SpParam = sqlCmd.Parameters.Add("@RoundOff", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.RoundOff;
                    SpParam = sqlCmd.Parameters.Add("@UserNarration", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.UserNarration;
                    SpParam = sqlCmd.Parameters.Add("@SortNumber", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.SortNumber;
                    SpParam = sqlCmd.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.DiscPer;
                    SpParam = sqlCmd.Parameters.Add("@VchTypeID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.VchTypeID;
                    SpParam = sqlCmd.Parameters.Add("@CCID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CCID;
                    SpParam = sqlCmd.Parameters.Add("@CurrencyID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CurrencyID;
                    SpParam = sqlCmd.Parameters.Add("@PartyAddress", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Int);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.UserID;
                    SpParam = sqlCmd.Parameters.Add("@AgentID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.AgentID;
                    SpParam = sqlCmd.Parameters.Add("@CashDiscount", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CashDiscount;
                    SpParam = sqlCmd.Parameters.Add("@DPerType_ManualCalc_Customer", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.DPerType_ManualCalc_Customer;
                    SpParam = sqlCmd.Parameters.Add("@NetAmount", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.NetAmount;
                    SpParam = sqlCmd.Parameters.Add("@RefNo", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.RefNo;
                    SpParam = sqlCmd.Parameters.Add("@CashPaid", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CashPaid;
                    SpParam = sqlCmd.Parameters.Add("@CardPaid", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CardPaid;
                    SpParam = sqlCmd.Parameters.Add("@blnWaitforAuthorisation", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.blnWaitforAuthorisation;
                    SpParam = sqlCmd.Parameters.Add("@UserIDAuth", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.UserIDAuth;
                    SpParam = sqlCmd.Parameters.Add("@BillTime", SqlDbType.DateTime);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.BillTime;
                    SpParam = sqlCmd.Parameters.Add("@StateID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.StateID;
                    SpParam = sqlCmd.Parameters.Add("@ImplementingStateCode", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ImplementingStateCode;
                    SpParam = sqlCmd.Parameters.Add("@GSTType", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@CGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CGSTTotal;
                    SpParam = sqlCmd.Parameters.Add("@SGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.SGSTTotal;
                    SpParam = sqlCmd.Parameters.Add("@IGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.IGSTTotal;
                    SpParam = sqlCmd.Parameters.Add("@PartyGSTIN", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@BillType", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.BillType;
                    SpParam = sqlCmd.Parameters.Add("@blnHold", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.blnHold;
                    SpParam = sqlCmd.Parameters.Add("@PriceListID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.PriceListID;
                    SpParam = sqlCmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTime);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.EffectiveDate;
                    SpParam = sqlCmd.Parameters.Add("@partyCode", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@Email", SqlDbType.VarChar);
                    SpParam.Value = "";
                    SpParam = sqlCmd.Parameters.Add("@TaxType", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.TaxType;
                    SpParam = sqlCmd.Parameters.Add("@QtyTotal", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.QtyTotal;
                    SpParam = sqlCmd.Parameters.Add("@DestCCID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.DestCCID;
                    SpParam = sqlCmd.Parameters.Add("@AgentCommMode", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.AgentCommMode;
                    SpParam = sqlCmd.Parameters.Add("@AgentCommAmount", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.AgentCommAmount;
                    SpParam = sqlCmd.Parameters.Add("@AgentLID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.AgentLID;
                    SpParam = sqlCmd.Parameters.Add("@BlnStockInsert", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.BlnStockInsert;
                    SpParam = sqlCmd.Parameters.Add("@BlnConverted", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.BlnConverted;
                    SpParam = sqlCmd.Parameters.Add("@ConvertedParentVchTypeID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ConvertedParentVchTypeID;
                    SpParam = sqlCmd.Parameters.Add("@ConvertedVchTypeID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ConvertedVchTypeID;
                    SpParam = sqlCmd.Parameters.Add("@ConvertedVchNo", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ConvertedVchNo;
                    SpParam = sqlCmd.Parameters.Add("@ConvertedVchID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ConvertedVchID;
                    SpParam = sqlCmd.Parameters.Add("@DeliveryNoteDetails", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.DeliveryNoteDetails;
                    SpParam = sqlCmd.Parameters.Add("@OrderDetails", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.OrderDetails;
                    SpParam = sqlCmd.Parameters.Add("@IntegrityStatus", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.IntegrityStatus;
                    SpParam = sqlCmd.Parameters.Add("@BalQty", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.BalQty;
                    SpParam = sqlCmd.Parameters.Add("@CustomerpointsSettled", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CustomerpointsSettled;
                    SpParam = sqlCmd.Parameters.Add("@blnCashPaid", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.blnCashPaid;
                    SpParam = sqlCmd.Parameters.Add("@originalsalesinvid", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.originalsalesinvid;
                    SpParam = sqlCmd.Parameters.Add("@retuninvid", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.retuninvid;
                    SpParam = sqlCmd.Parameters.Add("@returnamount", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.returnamount;
                    SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.SystemName;
                    SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.LastUpdateDate;
                    SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.LastUpdateTime;
                    SpParam = sqlCmd.Parameters.Add("@DeliveryDetails", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.DeliveryDetails;
                    SpParam = sqlCmd.Parameters.Add("@DespatchDetails", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.DespatchDetails;
                    SpParam = sqlCmd.Parameters.Add("@TermsOfDelivery", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.TermsOfDelivery;
                    SpParam = sqlCmd.Parameters.Add("@FloodCessTot", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.FloodCessTot;
                    SpParam = sqlCmd.Parameters.Add("@CounterID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CounterID;
                    SpParam = sqlCmd.Parameters.Add("@ExtraCharges", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ExtraCharges;
                    SpParam = sqlCmd.Parameters.Add("@ReferenceAutoNO", SqlDbType.VarChar);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.ReferenceAutoNO;
                    SpParam = sqlCmd.Parameters.Add("@CostFactor", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.CostFactor;
                    SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                    SpParam.Value = clsStockJournal.clsJsonPMInfo_.TenantID;
                    SpParam = sqlCmd.Parameters.Add("@JsonData", SqlDbType.VarChar);
                    SpParam.Value = strJson;
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
                return " - 1" + "| " + ex.Message;
            }
            finally
            {
                //sqlConn.Close();
            }
            //}
        }

        public string StockJournalDetailCRUD(clsJSonStockJournal clsStockJournal, SqlConnection sqlConn, SqlTransaction trans, string sBatchCode = "", int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sBatchCodeNew = "";
            DataTable dbtl = clsStockJournal.clsJsonSJDetailsInfoList_.ToDataTable();
            try
            {
                if (iAction == 0)
                {
                    for (int i = 0; i < dbtl.Rows.Count; i++)
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("UspStockJournalItemInsert", sqlConn, trans))
                        {
                            if (dbtl.Rows[i]["BatchCode"].ToString().Contains("@") == true)
                                sBatchCodeNew = dbtl.Rows[i]["BatchCode"].ToString().Substring(0, dbtl.Rows[i]["BatchCode"].ToString().IndexOf("@"));
                            else
                                sBatchCodeNew = dbtl.Rows[i]["BatchCode"].ToString();

                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                            DataSet dsDtl = new DataSet();
                            SqlParameter SpParam = new SqlParameter();

                            SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                            SpParam.Value = clsStockJournal.clsJsonPMInfo_.InvId; //Convert.ToDecimal(dbtl.Rows[i]["InvID"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@ItemId", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ItemId"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Qty"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@BalQty", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["BalQty"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@EnteredQty", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["EnteredQty"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@StockQty", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["StockQty"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Rate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Rate"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@UnitId", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["UnitId"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Batch", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["Batch"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@TaxPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["TaxPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["TaxAmount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Discount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["MRP"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@SlNo", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["SlNo"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Prate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Prate"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Free", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Free"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@SerialNos", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["SerialNos"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@ItemDiscount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ItemDiscount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                            SpParam.Value = sBatchCodeNew; //dbtl.Rows[i]["BatchCode"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@iCessOnTax", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["iCessOnTax"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@blnCessOnTax", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["blnCessOnTax"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Expiry", SqlDbType.DateTime);
                            SpParam.Value = Convert.ToDateTime(dbtl.Rows[i]["Expiry"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@ItemDiscountPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ItemDiscountPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@RateInclusive", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["RateInclusive"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@ITaxableAmount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ITaxableAmount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@INetAmount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["INetAmount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CGSTTaxPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@CGSTTaxAmt", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CGSTTaxAmt"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["SGSTTaxPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@SGSTTaxAmt", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["SGSTTaxAmt"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IGSTTaxPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IGSTTaxAmt", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IGSTTaxAmt"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@iRateDiscPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["iRateDiscPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@iRateDiscount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["iRateDiscount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["BatchUnique"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@blnQtyIN", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["blnQtyIN"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@CRate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CRate"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Unit", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["Unit"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@ItemStockID", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ItemStockID"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IcessPercent", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IcessPercent"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IcessAmt", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IcessAmt"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IQtyCompCessPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IQtyCompCessPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IQtyCompCessAmt", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IQtyCompCessAmt"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@StockMRP", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["StockMRP"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@BaseCRate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["BaseCRate"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@InonTaxableAmount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["InonTaxableAmount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@IAgentCommPercent", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["IAgentCommPercent"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@BlnDelete", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["BlnDelete"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Id", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Id"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@StrOfferDetails", SqlDbType.VarChar);
                            SpParam.Value = dbtl.Rows[i]["StrOfferDetails"].ToString();
                            SpParam = sqlCmd.Parameters.Add("@BlnOfferItem", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["BlnOfferItem"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@GrossAmount", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["GrossAmount"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@iFloodCessPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["iFloodCessPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@iFloodCessAmt", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["iFloodCessAmt"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate1"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate2"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate3"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate4"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate5"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Costrate", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Costrate"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@CostValue", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["CostValue"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Profit", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Profit"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@ProfitPer", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["ProfitPer"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@DiscMode", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["DiscMode"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate1Per", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate1Per"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate2Per", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate2Per"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate3Per", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate3Per"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate4Per", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate4Per"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Srate5Per", SqlDbType.Decimal);
                            SpParam.Value = Convert.ToDecimal(dbtl.Rows[i]["Srate5Per"].ToString());
                            SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                            SpParam.Value = iAction;

                            sqlDa.Fill(dsDtl);

                            dtResult = dsDtl.Tables[0];
                        }
                    }
                }
                else if (iAction == 2)
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspSalesItemInsert", sqlConn, trans))
                    {
                        if (dbtl.Rows[0]["BatchCode"].ToString().Contains("@") == true)
                            sBatchCodeNew = dbtl.Rows[0]["BatchCode"].ToString().Substring(0, dbtl.Rows[0]["BatchCode"].ToString().IndexOf("@"));

                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataSet dsDtl = new DataSet();
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["InvID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ItemId", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemId"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Qty"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Rate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Rate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@UnitId", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["UnitId"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Batch", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["Batch"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@TaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["TaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["TaxAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Discount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["MRP"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SlNo", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SlNo"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Prate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Prate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Free", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Free"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SerialNos", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["SerialNos"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@ItemDiscount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemDiscount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchCode"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@iCessOnTax", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iCessOnTax"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@blnCessOnTax", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["blnCessOnTax"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Expiry", SqlDbType.DateTime);
                        SpParam.Value = Convert.ToDateTime(dbtl.Rows[0]["Expiry"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ItemDiscountPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemDiscountPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@RateInclusive", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["RateInclusive"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ITaxableAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ITaxableAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@INetAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["INetAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CGSTTaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CGSTTaxAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SGSTTaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SGSTTaxAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IGSTTaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IGSTTaxAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iRateDiscPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iRateDiscPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iRateDiscount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iRateDiscount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchUnique"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@blnQtyIN", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["blnQtyIN"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CRate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CRate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Unit", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["Unit"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@ItemStockID", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemStockID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IcessPercent", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IcessPercent"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IcessAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IcessAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IQtyCompCessPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IQtyCompCessPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IQtyCompCessAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IQtyCompCessAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@StockMRP", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["StockMRP"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BaseCRate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BaseCRate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@InonTaxableAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["InonTaxableAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IAgentCommPercent", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IAgentCommPercent"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BlnDelete", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BlnDelete"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Id", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Id"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@StrOfferDetails", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["StrOfferDetails"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@BlnOfferItem", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BlnOfferItem"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BalQty", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BalQty"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@GrossAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["GrossAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iFloodCessPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iFloodCessPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iFloodCessAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iFloodCessAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Costrate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Costrate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CostValue", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CostValue"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Profit", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Profit"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ProfitPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ProfitPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@DiscMode", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["DiscMode"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate1Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate2Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;

                        sqlDa.Fill(dsDtl);
                        dtResult = dsDtl.Tables[0];
                    }
                }
                else if (iAction == 3)
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspSalesItemInsert", sqlConn, trans))
                    {
                        if (dbtl.Rows[0]["BatchCode"].ToString().Contains("@") == true)
                            sBatchCodeNew = dbtl.Rows[0]["BatchCode"].ToString().Substring(0, dbtl.Rows[0]["BatchCode"].ToString().IndexOf("@"));

                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataSet dsDtl = new DataSet();
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["InvID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ItemId", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemId"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Qty"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Rate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Rate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@UnitId", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["UnitId"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Batch", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["Batch"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@TaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["TaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["TaxAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Discount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["MRP"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SlNo", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SlNo"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Prate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Prate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Free", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Free"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SerialNos", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["SerialNos"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@ItemDiscount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemDiscount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchCode"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@iCessOnTax", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iCessOnTax"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@blnCessOnTax", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["blnCessOnTax"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Expiry", SqlDbType.DateTime);
                        SpParam.Value = Convert.ToDateTime(dbtl.Rows[0]["Expiry"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ItemDiscountPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemDiscountPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@RateInclusive", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["RateInclusive"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ITaxableAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ITaxableAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@INetAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["INetAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CGSTTaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CGSTTaxAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SGSTTaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SGSTTaxAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IGSTTaxPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IGSTTaxAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iRateDiscPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iRateDiscPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iRateDiscount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iRateDiscount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["BatchUnique"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@blnQtyIN", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["blnQtyIN"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CRate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CRate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Unit", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["Unit"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@ItemStockID", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemStockID"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IcessPercent", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IcessPercent"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IcessAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IcessAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IQtyCompCessPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IQtyCompCessPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IQtyCompCessAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IQtyCompCessAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@StockMRP", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["StockMRP"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BaseCRate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BaseCRate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@InonTaxableAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["InonTaxableAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@IAgentCommPercent", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IAgentCommPercent"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BlnDelete", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BlnDelete"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Id", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Id"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@StrOfferDetails", SqlDbType.VarChar);
                        SpParam.Value = dbtl.Rows[0]["StrOfferDetails"].ToString();
                        SpParam = sqlCmd.Parameters.Add("@BlnOfferItem", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BlnOfferItem"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@BalQty", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BalQty"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@GrossAmount", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["GrossAmount"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iFloodCessPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iFloodCessPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@iFloodCessAmt", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iFloodCessAmt"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Costrate", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Costrate"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@CostValue", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CostValue"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Profit", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Profit"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@ProfitPer", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ProfitPer"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@DiscMode", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["DiscMode"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate1Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate2Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate3Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate4Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Srate5Per", SqlDbType.Decimal);
                        SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5Per"].ToString());
                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;

                        sqlDa.Fill(dsDtl);
                        dtResult = dsDtl.Tables[0];
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

        public string PurStockInsert(Hashtable hstStk, string sActon = "")
        {
            string strBatchCode = "";
            strBatchCode = Comm.StockInsert(sActon, Convert.ToDecimal(hstStk["ItemID"].ToString()), hstStk["BatchCode"].ToString(), Convert.ToDecimal(hstStk["Qty"].ToString()), Convert.ToDecimal(hstStk["MRP"].ToString()), Convert.ToDecimal(hstStk["CostRateInc"].ToString()), Convert.ToDecimal(hstStk["CostRateExcl"].ToString()), Convert.ToDecimal(hstStk["PRateExcl"].ToString()), Convert.ToDecimal(hstStk["PrateInc"].ToString()), Convert.ToDecimal(hstStk["TaxPer"].ToString()), Convert.ToDecimal(hstStk["SRate1"].ToString()), Convert.ToDecimal(hstStk["SRate2"].ToString()), Convert.ToDecimal(hstStk["SRate3"].ToString()), Convert.ToDecimal(hstStk["SRate4"].ToString()), Convert.ToDecimal(hstStk["SRate5"].ToString()), Convert.ToInt32(hstStk["BatchMode"].ToString()), hstStk["VchType"].ToString(), Convert.ToDateTime(hstStk["VchDate"].ToString()), Convert.ToDateTime(hstStk["ExpDt"].ToString()), Convert.ToDouble(hstStk["RefID"].ToString()), Convert.ToDouble(hstStk["VchTypeID"].ToString()), Convert.ToDouble(hstStk["CCID"].ToString()), Convert.ToDouble(hstStk["TenantID"].ToString()));
            string[] sData = strBatchCode.Split('|');
            if (sData.Length > 0)
                strBatchCode = sData[0].ToString();

            return strBatchCode;
        }

        //public DataTable GetSalesMaster(UspGetSalesInfo Info, bool blnIsPrevNext = false)
        //{
        //    DataTable dtbl = new DataTable();
        //    try
        //    {
        //        using (var sqlcon = GetDBConnection())
        //        {
        //            using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetSalesMaster", sqlcon))
        //            {
        //                sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
        //                sqlda.SelectCommand.Parameters.Add("@InvId", SqlDbType.Decimal).Value = Info.InvId;
        //                sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
        //                sqlda.SelectCommand.Parameters.Add("@VchTypeID", SqlDbType.Decimal).Value = Info.VchTypeID;
        //                sqlda.SelectCommand.Parameters.Add("@blnPrevNext", SqlDbType.Bit).Value = blnIsPrevNext;
        //                sqlda.Fill(dtbl);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //    }

        //    return dtbl;
        //}

        //public DataTable GetSalesDetailItem(UspGetSalesInfo Info)
        //{
        //    DataTable dtbl = new DataTable();
        //    try
        //    {
        //        using (var sqlcon = GetDBConnection())
        //        {
        //            using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetSalesDetailItem", sqlcon))
        //            {
        //                sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
        //                sqlda.SelectCommand.Parameters.Add("@InvID", SqlDbType.Decimal).Value = Info.InvId;
        //                sqlda.Fill(dtbl);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //    }

        //    return dtbl;
        //}
        public DataTable GetStockJournalMaster(UspGetStockJournalInfo Info, bool blnIsPrevNext = false)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStockJournalMaster", sqlcon))
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

        public DataTable GetStockJournalDetailItem(UspGetStockJournalInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStockJournalDetailItem", sqlcon))
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
